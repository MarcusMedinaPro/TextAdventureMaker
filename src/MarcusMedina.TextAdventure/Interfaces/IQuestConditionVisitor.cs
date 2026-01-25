using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IQuestConditionVisitor
{
    bool Visit(HasItemCondition condition);
    bool Visit(NpcStateCondition condition);
    bool Visit(AllOfCondition condition);
    bool Visit(AnyOfCondition condition);
}
