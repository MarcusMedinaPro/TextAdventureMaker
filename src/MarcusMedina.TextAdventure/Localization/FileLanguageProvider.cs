// <copyright file="FileLanguageProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Localization;

using System.Globalization;

public sealed class FileLanguageProvider : ILanguageProvider
{
    private readonly Dictionary<string, string> _entries = new(StringComparer.OrdinalIgnoreCase);

    public FileLanguageProvider(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        Load(path);
    }

    public string Get(string key) => string.IsNullOrWhiteSpace(key) ? "" : _entries.TryGetValue(key, out var value) ? value : $"[[{key}]]";

    public string Format(string key, params object[] args)
    {
        var template = Get(key);
        return string.Format(CultureInfo.InvariantCulture, template, args);
    }

    private void Load(string path)
    {
        foreach (var rawLine in File.ReadAllLines(path))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;
            if (line.StartsWith("#", StringComparison.Ordinal))
                continue;
            if (line.StartsWith("//", StringComparison.Ordinal))
                continue;

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
                continue;

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();

            if (key.Length == 0)
                continue;
            value = value.Replace("\\n", "\n");
            _entries[key] = value;
        }
    }
}
