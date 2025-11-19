namespace Metaballs;

static class MetaballsConfig
{
	public static int GridResolution { get; } = 1;
	public static bool PrintSamples { get; } = false;
	public static bool ShowGrid { get; } = false;
	public static bool DrawCircles { get; } = true;
	public static bool FillRects { get; } = false;
	public static bool ShowCorners { get; } = false;
	public static int NumBlobs { get; } = 10;
	public static int MinRadius { get; } = 16;
	public static int MaxRadius { get; } = 32;
	public static bool Interpolated { get; } = true;
}
