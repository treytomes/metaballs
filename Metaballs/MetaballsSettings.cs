namespace Metaballs;

class MetaballsSettings
{
	public int GridResolution { get; } = 1;
	public bool PrintSamples { get; } = false;
	public bool ShowGrid { get; } = false;
	public bool DrawCircles { get; } = true;
	public bool FillRects { get; } = false;
	public bool ShowCorners { get; } = false;
	public int NumBlobs { get; } = 10;
	public int MinRadius { get; } = 16;
	public int MaxRadius { get; } = 32;
	public bool Interpolated { get; } = true;
}
