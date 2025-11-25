using Metaballs.Props;
using OpenTK.Mathematics;
using System.Drawing;

namespace Metaballs;

class BlobFactory(MetaballsAppSettings settings)
{
	public Blob CreateRandomBlob(Rectangle bounds)
	{
		var r = (int)Math.Floor(Random.Shared.NextSingle() * (settings.Metaballs.MaxRadius - settings.Metaballs.MinRadius) + settings.Metaballs.MinRadius);
		var x = bounds.Left + (float)Math.Floor(Random.Shared.NextSingle() * (bounds.Width - r * 2) + r);
		var y = bounds.Top + (float)Math.Floor(Random.Shared.NextSingle() * (bounds.Height - r * 2) + r);

		var v = settings.Metaballs.InitialDrift
			? new Vector2(
				(Random.Shared.NextSingle() * 3 - 2) * Random.Shared.NextSingle() * 8 + 8,
				(Random.Shared.NextSingle() * 3 - 2) * Random.Shared.NextSingle() * 8 + 8
			)
			: Vector2.Zero;

		var blob = new Blob(new Vector2(x, y), r, v)
		{
			DrawOutline = settings.Debug,
		};
		return blob;
	}

	public Blob CreateRadialBlob(Vector2 position, CreateRadialBlobProps props)
	{
		var radius = Random.Shared.Next(props.MinRadius, props.MaxRadius);
		var angle = Random.Shared.Next(0, 360) * Math.PI / 180.0f;
		var offset = new Vector2(radius * (float)Math.Cos(angle), radius * (float)Math.Sin(angle));
		return new Blob(position + offset, radius)
		{
			DrawOutline = settings.Debug,
		};
	}
}