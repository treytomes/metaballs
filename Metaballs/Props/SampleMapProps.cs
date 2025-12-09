using RetroTK.Gfx;

namespace Metaballs.Props;

record SampleMapProps
{
	public RadialColor OutlineColor { get; init; } = RadialColor.Yellow;
	public RadialColor FillColor { get; init; } = RadialColor.Gray.Lerp(RadialColor.Black, 0.5f);

	public bool IsOutlined { get; init; } = true;
	public bool IsFilled { get; init; } = true;
}
