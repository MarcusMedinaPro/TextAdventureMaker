namespace MarcusMedina.TextAdventure.DSLHelper;

internal static class DslHelperAiPrompts
{
    public static string BuildDescriptionSystemPrompt(string entityType)
    {
        return
            $"""
            You improve text-adventure {entityType} descriptions.
            Rules:
            - Write in British English.
            - Keep lore coherent with the user instruction.
            - Return only the improved description text.
            - No markdown, no JSON, no labels.
            - Use one concise paragraph.
            """;
    }

    public static string BuildDescriptionInput(
        string entityType,
        string entityId,
        string instruction,
        string worldContext)
    {
        return
            $"""
            Entity type: {entityType}
            Entity id: {entityId}
            User draft: {instruction}

            World context:
            {worldContext}
            """;
    }

    public static string BuildActionPlannerSystemPrompt()
    {
        return
            """
            You convert user worldbuilding instructions into compact JSON actions for a text-adventure DSL editor.
            Return one single-line JSON object only, with this shape:
            {"actions":[{"action":"create_room","room_id":"entry"}]}

            Supported action values:
            - create_room
            - add_door
            - add_item
            - add_npc
            - describe_room
            - describe_item
            - describe_npc
            - describe_door
            - move_to
            - delete_room
            - delete_item
            - delete_npc
            - delete_door
            - none

            Field guidance:
            - For create_room: room_id, optional description.
            - For add_door: from_id, to_id, optional direction, door_id, door_name, description.
            - For add_item: room_id, item_id, optional item_name, description.
            - For add_npc: room_id, npc_id, optional npc_name, description.
            - For describe_*: target id and description.
            - For describe_door: door_id and description.
            - For delete_*: provide the matching id field.
            - Use "this" for current room if needed.
            - If request is unclear, use one action: {"action":"none","reason":"..."}.
            - Keep all descriptions in British English.
            """;
    }

    public static string BuildActionPlannerInput(string userInstruction, string worldContext)
    {
        return
            $"""
            Instruction: {userInstruction}

            World context:
            {worldContext}
            """;
    }
}
