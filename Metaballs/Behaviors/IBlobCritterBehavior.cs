using RetroTK;
using RetroTK.Events;

namespace Metaballs.Behaviors;

interface IBlobCritterBehavior : IEventHandler
{
	BlobCritter Owner { get; }
	void Update(GameTime gameTime);
}
