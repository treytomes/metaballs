using OpenTK.Mathematics;
using RetroTK.Gfx;

namespace Metaballs;

class BlobFactory(IRenderingContext rc)
{
	private int _displayWidth = rc.Width;
	private int _displayHeight = rc.Height;

	public Blob CreateRandomBlob()
	{
		var r = (int)Math.Floor(Random.Shared.NextSingle() * (MetaballsConfig.MaxRadius - MetaballsConfig.MinRadius) + MetaballsConfig.MinRadius);
		var x = (float)Math.Floor(Random.Shared.NextSingle() * (_displayWidth - r * 2) + r);
		var y = (float)Math.Floor(Random.Shared.NextSingle() * (_displayHeight - r * 2) + r);
		return new Blob(new Vector2(x, y), r);
	}
}