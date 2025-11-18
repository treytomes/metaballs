namespace Metaballs.Brushes;

class NoisyCircleMetaballsBrush : IFireBrush
{
	public void Draw(FireBuffer buffer, int sx, int sy, int size)
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
						var color = Random.Shared.NextSingle();
						buffer[y, x] = color;
					}
				}
			}
		}
	}
}
