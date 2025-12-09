using Metaballs.Behaviors.Props;

namespace Metaballs.Props;

record CreateBlobCritterProps
{
	public int MinNumBlobs { get; init; } = 3;
	public int MaxNumBlobs { get; init; } = 5;

	public float MinSpeed { get; init; } = 150f;
	public float MaxSpeed { get; init; } = 250f;

	// Friction must be < 5 or so, because multiplied by dt (~0.016).
	public float MinFriction { get; init; } = 0.5f;
	public float MaxFriction { get; init; } = 2f;

	public float StretchScale { get; init; } = 1f;

	public CreateRadialBlobProps BlobProps { get; init; } = new()
	{
		MinRadius = 8,
		MaxRadius = 16,
	};

	public List<BlobCritterBehaviorProps> Behaviors { get; init; } = new()
	{
		new MouseFollowingBlobCritterBehaviorProps()
		{
			MaxAcceleration = 50f,
			MaxSpeed = 200f,
			Responsiveness = 0.5f,
			SpeedDamping = 0.05f,
		},
	};
}
