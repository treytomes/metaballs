using Metaballs.Behaviors.Props;

namespace Metaballs.Behaviors;

class BehaviorFactory
{
	public IBlobCritterBehavior Create(BlobCritter owner, BlobCritterBehaviorProps props)
	{
		return props switch
		{
			MouseFollowingBlobCritterBehaviorProps p => Create(owner, p),
			_ => throw new ArgumentException($"Unknown prop type: {props.GetType().Name}", nameof(props)),
		};
	}

	public IBlobCritterBehavior Create(BlobCritter owner, MouseFollowingBlobCritterBehaviorProps props)
	{
		return new MouseFollowingBlobCritterBehavior(owner, props);
	}
}