using RetroTK.States;

namespace RetroTK.Events;

/// <summary>  
/// Provides data for the StateTransitioned event, which occurs after a transition has completed.
/// </summary>  
class StateTransitionEventArgs(TransitionType transitionType, IGameState? previousState, IGameState? newState) : EventArgs
{
	/// <summary>  
	/// Gets the type of transition that occurred.  
	/// </summary>  
	public TransitionType TransitionType => transitionType;

	/// <summary>  
	/// Gets the previous state, if any.  
	/// </summary>  
	public IGameState? PreviousState => previousState;

	/// <summary>  
	/// Gets the new state, if any.  
	/// </summary>  
	public IGameState? NewState => newState;
}