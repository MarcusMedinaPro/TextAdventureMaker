namespace MarcusMedina.TextAdventure.Interfaces;

public interface IStats
{
    int Health { get; }
    int MaxHealth { get; }
    void Damage(int amount);
    void Heal(int amount);
    void SetMaxHealth(int maxHealth);
    void SetHealth(int health);
}
