namespace Metaballs;

class MetaballsSettings
{
	public int GridResolution { get; set; } = 1;
	public bool PrintSamples { get; set; } = false;
	public bool ShowGrid { get; set; } = false;
	public bool DrawCircles { get; set; } = true;
	public bool FillRects { get; set; } = false;
	public bool ShowCorners { get; set; } = false;
	public int NumBlobs { get; set; } = 10;
	public int MinRadius { get; set; } = 16;
	public int MaxRadius { get; set; } = 32;
	public bool Interpolated { get; set; } = true;
	public bool InitialDrift { get; set; } = true;
}
