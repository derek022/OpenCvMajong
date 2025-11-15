using Mahjong.Core;

namespace Mahjong.Resolution.SearchState;

public interface ISearchLogic
{
    void Initialize(GameLogic logic);
    Task<LinkedList<GameLogic>> SearchState();
}