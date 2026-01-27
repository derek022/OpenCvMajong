namespace SoluCore.Game;

public interface IGameState
{
    bool IsSameState( IGameState state);
    IAction CurAction { get; set; }
}