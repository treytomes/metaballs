namespace Metaballs.Brushes;

class BlobbyCircleMetaballsBrush : IMetaballsBrush
{
	// Note: `init` = the property can be modified as part of the instance initializer block, but not after.
	public float Flicker { get; init; } = 0.7f;
	public float Blend { get; init; } = 0.3f;

	public void Draw(MetaballsBuffer buffer, int sx, int sy, int size)
	{
		var sizeSq = size * size;
		for (var dy = -size; dy <= size; dy++)
		{
			for (var dx = -size; dx <= size; dx++)
			{
				var x = sx + dx;
				var y = sy + dy;
				if (0 <= x && x < buffer.Width && 0 <= y && y < buffer.Height)
				{
					if (dx * dx + dy * dy < sizeSq)
					{
						// Note: The inverse scaled radius puts the brightest colors in the middle of the blob.
						// The flicker factor will allow the colors to mush throughout the blob by a random amount per frame.
						var prevColor = buffer[y, x];
						var nextColor = (float)(1.0f - Math.Sqrt(dx * dx + dy * dy) / size) * (Random.Shared.NextSingle() * Flicker + (1.0f - Flicker));

						// Note: Using the blend factor helps the brush look nicer as the user is drawing.
						// Applying the flicker factor to the blend looks even nicer.
						var blendFactor = Blend * Flicker;
						var color = prevColor * (1.0f - blendFactor) + nextColor * blendFactor;
						buffer[y, x] = color;
					}
				}
			}
		}
	}
}