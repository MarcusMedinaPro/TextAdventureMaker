using MarcusMedina.TextAdventure.Extensions;
using System.Linq;

namespace MarcusMedina.TextAdventure.Models;

public sealed class GameItemList
{
    private readonly Dictionary<string, Item> _items = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<Item> Items => _items.Values;

    public Item Add(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return Add(new Item(name.ToId(), name));
    }

    public Item Add(string id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return Add(new Item(id, name));
    }

    public Item Add(string id, string name, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return Add(new Item(id, name, description));
    }

    public Item Add(Item item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items[item.Id] = item;
        return item;
    }

    public GameItemList AddMany(params string[] names)
    {
        if (names == null) return this;
        foreach (var name in names)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Add(name);
            }
        }
        return this;
    }

    public GameItemList AddMany(IEnumerable<string> names)
    {
        if (names == null) return this;
        foreach (var name in names)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Add(name);
            }
        }
        return this;
    }

    public Item? Find(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        if (_items.TryGetValue(token, out var item)) return item;
        return _items.Values.FirstOrDefault(i => i.Matches(token));
    }

    public Item Get(string token)
    {
        var item = Find(token);
        if (item == null)
        {
            throw new KeyNotFoundException($"No item found for '{token}'.");
        }
        return item;
    }

    public bool TryGet(string token, out Item item)
    {
        item = Find(token) ?? null!;
        return item != null;
    }

    public bool Remove(string token)
    {
        var item = Find(token);
        if (item == null) return false;
        return _items.Remove(item.Id);
    }

    public void Clear() => _items.Clear();

    public Item this[string token] => Get(token);

    public Item Call(string token) => Get(token);
}
