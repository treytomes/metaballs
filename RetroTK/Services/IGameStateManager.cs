using RetroTK.Events;
using RetroTK.States;

namespace RetroTK.Services;

interface IGameStateManager : IGameComponent, IEventHandler, IDisposable
{
	bool HasState { get; }
	IGameState? CurrentState { get; }
	void EnterState(IGameState state);
	bool LeaveState();
}
