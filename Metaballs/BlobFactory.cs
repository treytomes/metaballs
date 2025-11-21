using System.Drawing;
using OpenTK.Mathematics;

namespace Metaballs;

class BlobFactory(MetaballsSettings settings)
{
	public Blob CreateRandomBlob(Rectangle bounds)
	{
		var r = (int)Math.Floor(Random.Shared.NextSingle() * (settings.MaxRadius - settings.MinRadius) + settings.MinRadius);
		var x = bounds.Left + (float)Math.Floor(Random.Shared.NextSingle() * (bounds.Width - r * 2) + r);
		var y = bounds.Top + (float)Math.Floor(Random.Shared.NextSingle() * (bounds.Height - r * 2) + r);

		var v = settings.InitialDrift
			? new Vector2(
				(Random.Shared.NextSingle() * 3 - 2) * Random.Shared.NextSingle() * 8 + 8,
				(Random.Shared.NextSingle() * 3 - 2) * Random.Shared.NextSingle() * 8 + 8
			)
			: Vector2.Zero;

		var blob = new Blob(new Vector2(x, y), r, v)
		{
			DrawOutline = settings.DrawCircles,
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
			DrawOutline = props.DrawOutline,
		};
	}
}