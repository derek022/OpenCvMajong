using SoluCore.Game;

namespace SoluCore.SearchState;

public interface ISearchLogic
{
    void Initialize(IGameLogic logic);
    Task<LinkedList<IGameLogic>?> SearchState();
    
    bool IsProcessedState(LinkedList<IGameLogic>? input,IGameLogic next);
    
    bool IsMatchDead(LinkedList<IGameLogic>? deadList,IGameLogic next);
}