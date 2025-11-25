namespace Metaballs.Props;

record CreateBlobCritterProps
{
	public int MinNumBlobs { get; init; } = 3;
	public int MaxNumBlobs { get; init; } = 5;
	public float MinSpeed { get; init; } = 10f;
	public float MaxSpeed { get; init; } = 20f;
	public float MinFriction { get; init; } = 200f;
	public float MaxFriction { get; init; } = 400f;

	/// <summary>
	/// How far can the critter stretch beyond it's base radius?
	/// </summary>
	public float StretchScale { get; init; } = 1.5f;

	public CreateRadialBlobProps BlobProps { get; init; } = new()
	{
		MinRadius = 8,
		MaxRadius = 16,
		DrawOutline = true
	};
}
