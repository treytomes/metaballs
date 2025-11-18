using RetroTK.States;

namespace RetroTK.Services;

public interface IGameEngine
{
	Task RunAsync<TGameState>(CancellationToken cancellationToken = default)
		where TGameState : IGameState;
}