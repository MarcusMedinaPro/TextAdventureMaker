namespace MarcusMedina.TextAdventure.Interfaces;

public interface IQuestCondition
{
    bool Accept(IQuestConditionVisitor visitor);
}
