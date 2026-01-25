using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Extensions;

public static class GameEntityExtensions
{
    /// <summary>
    /// Sets an arbitrary string property on any game entity.
    /// </summary>
    public static T SetProperty<T>(this T entity, string key, string value) where T : IGameEntity
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (string.IsNullOrWhiteSpace(key)) return entity;
        entity.Properties[key.Trim()] = value ?? "";
        return entity;
    }

    /// <summary>
    /// Gets an arbitrary string property from a game entity.
    /// </summary>
    public static string? GetProperty(this IGameEntity entity, string key)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (string.IsNullOrWhiteSpace(key)) return null;
        return entity.Properties.TryGetValue(key.Trim(), out var value) ? value : null;
    }

    /// <summary>
    /// Sets a hint for a game entity.
    /// </summary>
    public static T SetHint<T>(this T entity, string text) where T : IGameEntity
    {
        return entity.SetProperty("hint", text);
    }

    /// <summary>
    /// Gets a hint for a game entity.
    /// </summary>
    public static string? GetHint(this IGameEntity entity)
    {
        return entity.GetProperty("hint");
    }
}
