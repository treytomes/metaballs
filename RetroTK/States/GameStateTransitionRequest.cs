namespace RetroTK.States;

/// <summary>
/// Represents a request to transition between game states.
/// </summary>
public class GameStateTransitionRequest(TransitionType type, IGameState? targetState)
{
	/// <summary>
	/// Gets the type of transition requested.
	/// </summary>
	public TransitionType Type => type;

	/// <summary>
	/// Gets the target state for the transition, if applicable.
	/// </summary>
	public IGameState? TargetState => targetState;
}