namespace Metaballs.Props;

record CreateRadialBlobProps
{
	public int MinRadius { get; init; } = 8;
	public int MaxRadius { get; init; } = 16;
	public bool DrawOutline { get; init; } = false;
}
