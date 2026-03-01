namespace MarcusMedina.TextAdventure.DSLHelper;

internal sealed record DslAiTextResult(
    bool Success,
    string Text,
    string Provider,
    string? Error = null);

internal sealed record DslAiPlanResult(
    bool Success,
    IReadOnlyList<DslAiAction> Actions,
    string RawResponse,
    string Provider,
    string? Error = null);

internal sealed record DslAiAction(
    string Action,
    string? RoomId = null,
    string? FromId = null,
    string? ToId = null,
    string? Direction = null,
    string? DoorId = null,
    string? DoorName = null,
    string? ItemId = null,
    string? ItemName = null,
    string? NpcId = null,
    string? NpcName = null,
    string? Description = null,
    string? Reason = null);
