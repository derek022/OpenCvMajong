namespace SoluCore.Game;

public interface IGameLogic
{
    bool IsFinalState();
    void PrintState();
    
    void SetAction(IAction action);
    bool IsCanDoAction(IAction action);
    bool DoAction(IAction action);

    IGameLogic DeepClone();
    
    IGameState Stage { get; set; }

    IEnumerable<IAction> GetActions();
}