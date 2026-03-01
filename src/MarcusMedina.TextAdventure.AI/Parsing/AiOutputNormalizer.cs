// <copyright file="AiOutputNormalizer.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json;
using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.AI.Parsing;

public static class AiOutputNormalizer
{
    public static string? NormalizeSingleCommand(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        string cleaned = text
            .Replace("```", string.Empty, StringComparison.Ordinal)
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Trim();

        string? firstLine = cleaned
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        if (string.IsNullOrWhiteSpace(firstLine))
            return null;

        int colonIndex = firstLine.IndexOf(':');
        if (colonIndex > 0 && colonIndex < firstLine.Length - 1)
        {
            string key = firstLine[..colonIndex].Trim();
            if (key.TextCompare("assistant") || key.TextCompare("command") || key.TextCompare("result"))
                firstLine = firstLine[(colonIndex + 1)..].Trim();
        }

        return firstLine.CollapseRepeats().Trim();
    }

    public static string? ExtractCommandText(JsonElement root)
    {
        return NormalizeSingleCommand(
            TryGetFirstString(root, "response", "output_text", "text", "result", "output", "answer")
            ?? TryGetOpenAiResponseOutputText(root)
            ?? TryGetChoiceMessageContent(root)
            ?? TryGetChoiceText(root)
            ?? TryGetGeminiCandidateText(root)
            ?? TryGetNestedDataText(root));
    }

    public static AiTokenUsage? ExtractTokenUsage(JsonElement root)
    {
        JsonElement usage = default;

        if (!TryGetPropertyIgnoreCase(root, "usage", out usage))
            return null;

        int? inputTokens =
            TryReadInt(usage, "prompt_tokens")
            ?? TryReadInt(usage, "input_tokens")
            ?? TryReadInt(usage, "promptTokenCount");

        int? outputTokens =
            TryReadInt(usage, "completion_tokens")
            ?? TryReadInt(usage, "output_tokens")
            ?? TryReadInt(usage, "candidatesTokenCount");

        return inputTokens is null && outputTokens is null
            ? null
            : new AiTokenUsage(inputTokens, outputTokens);
    }

    private static string? TryGetOpenAiResponseOutputText(JsonElement root)
    {
        if (!TryGetPropertyIgnoreCase(root, "output", out JsonElement output) || output.ValueKind != JsonValueKind.Array)
            return null;

        foreach (JsonElement item in output.EnumerateArray())
        {
            if (!TryGetPropertyIgnoreCase(item, "content", out JsonElement content) || content.ValueKind != JsonValueKind.Array)
                continue;

            foreach (JsonElement contentItem in content.EnumerateArray())
            {
                if (TryGetPropertyIgnoreCase(contentItem, "text", out JsonElement textValue) && textValue.ValueKind == JsonValueKind.String)
                    return textValue.GetString();
            }
        }

        return null;
    }

    private static string? TryGetChoiceMessageContent(JsonElement root)
    {
        if (!TryGetPropertyIgnoreCase(root, "choices", out JsonElement choices) || choices.ValueKind != JsonValueKind.Array)
            return null;

        JsonElement first = choices.EnumerateArray().FirstOrDefault();
        if (first.ValueKind == JsonValueKind.Undefined)
            return null;

        if (!TryGetPropertyIgnoreCase(first, "message", out JsonElement message) || message.ValueKind != JsonValueKind.Object)
            return null;

        if (!TryGetPropertyIgnoreCase(message, "content", out JsonElement content))
            return null;

        return content.ValueKind switch
        {
            JsonValueKind.String => content.GetString(),
            JsonValueKind.Array => content.EnumerateArray()
                .Select(item => TryGetPropertyIgnoreCase(item, "text", out JsonElement text) && text.ValueKind == JsonValueKind.String ? text.GetString() : null)
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)),
            _ => null
        };
    }

    private static string? TryGetChoiceText(JsonElement root)
    {
        if (!TryGetPropertyIgnoreCase(root, "choices", out JsonElement choices) || choices.ValueKind != JsonValueKind.Array)
            return null;

        JsonElement first = choices.EnumerateArray().FirstOrDefault();
        if (first.ValueKind == JsonValueKind.Undefined)
            return null;

        return TryGetPropertyIgnoreCase(first, "text", out JsonElement textValue) && textValue.ValueKind == JsonValueKind.String
            ? textValue.GetString()
            : null;
    }

    private static string? TryGetGeminiCandidateText(JsonElement root)
    {
        if (!TryGetPropertyIgnoreCase(root, "candidates", out JsonElement candidates) || candidates.ValueKind != JsonValueKind.Array)
            return null;

        JsonElement first = candidates.EnumerateArray().FirstOrDefault();
        if (first.ValueKind == JsonValueKind.Undefined)
            return null;

        if (!TryGetPropertyIgnoreCase(first, "content", out JsonElement content) || content.ValueKind != JsonValueKind.Object)
            return null;

        if (!TryGetPropertyIgnoreCase(content, "parts", out JsonElement parts) || parts.ValueKind != JsonValueKind.Array)
            return null;

        foreach (JsonElement part in parts.EnumerateArray())
        {
            if (TryGetPropertyIgnoreCase(part, "text", out JsonElement textValue) && textValue.ValueKind == JsonValueKind.String)
                return textValue.GetString();
        }

        return null;
    }

    private static string? TryGetNestedDataText(JsonElement root)
    {
        if (!TryGetPropertyIgnoreCase(root, "data", out JsonElement data) || data.ValueKind != JsonValueKind.Object)
            return null;

        return TryGetFirstString(data, "text", "output", "result", "response", "answer");
    }

    private static string? TryGetFirstString(JsonElement root, params string[] names)
    {
        foreach (string name in names)
        {
            if (!TryGetPropertyIgnoreCase(root, name, out JsonElement value))
                continue;

            if (value.ValueKind == JsonValueKind.String)
                return value.GetString();

            if (value.ValueKind == JsonValueKind.Array)
            {
                string? candidate = value
                    .EnumerateArray()
                    .Select(item => item.ValueKind == JsonValueKind.String ? item.GetString() : null)
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

                if (!string.IsNullOrWhiteSpace(candidate))
                    return candidate;
            }
        }

        return null;
    }

    private static int? TryReadInt(JsonElement root, string propertyName)
    {
        if (!TryGetPropertyIgnoreCase(root, propertyName, out JsonElement value))
            return null;

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int result))
            return result;

        if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out int parsed))
            return parsed;

        return null;
    }

    private static bool TryGetPropertyIgnoreCase(JsonElement root, string propertyName, out JsonElement value)
    {
        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty property in root.EnumerateObject())
            {
                if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }
}
