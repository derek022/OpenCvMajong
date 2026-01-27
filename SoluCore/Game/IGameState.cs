namespace SoluCore.Game;

public interface IGameState
{
    bool IsSameState( IGameState state);
    bool IsDeadMatchSimilarState( IGameState state );
    IAction CurAction { get; set; }
}