namespace MarcusMedina.TextAdventure.Interfaces;

public interface IContainer<T>
{
    IReadOnlyList<T> Contents { get; }
    bool CanAdd(T item);
    bool Add(T item);
    bool Remove(T item);
}
