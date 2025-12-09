using Metaballs.Fire.Particles;
using OpenTK.Mathematics;
using RetroTK;
using RetroTK.Gfx;

namespace Metaballs.Fire;

class ConsumableTile
{
	#region Constants

	private const int DEFAULT_RENDER_DELAY_MS = 0;
	private const int DEFAULT_CONSUMPTION_DELAY_MS = 1000;
	private const int DEFAULT_CONSUMPTION_RATE_MS = 500;

	// Note: Randomizing the consumption rate.
	private const float DEFAULT_NOISE_FACTOR = 0.75f;

	#endregion

	#region Fields

	private readonly RadialColor _fgColor;
	private readonly TimeSpan _consumptionRate;
	private TimeSpan _elapsedConsumption = TimeSpan.Zero;
	private TimeSpan _consumptionDelay;
	private TimeSpan _renderDelay;
	private int _consumptionOffset = 0;
	private readonly Vector2 _position;
	private readonly int _tileIndex;

	#endregion

	#region Constructors

	public ConsumableTile(int tileIndex, Vector2 position, RadialColor fgColor, TimeSpan? consumptionDelay = null, TimeSpan? consumptionRate = null, TimeSpan? renderDelay = null)
	{
		_fgColor = fgColor;
		_tileIndex = tileIndex;
		_position = position;
		_consumptionDelay = consumptionDelay ?? TimeSpan.FromMilliseconds(DEFAULT_CONSUMPTION_DELAY_MS);
		_consumptionRate = consumptionRate ?? TimeSpan.FromMilliseconds(DEFAULT_CONSUMPTION_RATE_MS);
		_renderDelay = renderDelay ?? TimeSpan.FromMilliseconds(DEFAULT_RENDER_DELAY_MS);
	}

	#endregion

	#region Methods

	public void Render(IRenderingContext rc, GlyphSet<Bitmap> tiles)
	{
		if (_renderDelay > TimeSpan.Zero) return;

		var x = (int)_position.X;
		var y = (int)_position.Y;
		var tile = tiles[_tileIndex];
		if (tile == null) return;

		for (var dy = tile.Height - 1; dy >= _consumptionOffset; dy--)
		{
			if (y + dy < 0)
			{
				continue;
			}
			if (y + dy >= rc.Height)
			{
				break;
			}
			for (var dx = 0; dx < tile.Width; dx++)
			{
				if (x + dx < 0)
				{
					continue;
				}
				if (x + dx >= rc.Width)
				{
					break;
				}
				var value = tile.GetPixel(dx, dy);
				if (value)
				{
					rc.SetPixel(x + dx, y + dy, _fgColor.Index);
				}
			}
		}
	}

	public void Update(GameTime gameTime, GlyphSet<Bitmap> tiles, ParticleFountain fountain)
	{
		if (tiles == null) return;

		_renderDelay = _renderDelay - gameTime.ElapsedTime;
		if (_renderDelay > TimeSpan.Zero) return;

		_consumptionDelay = _consumptionDelay - gameTime.ElapsedTime * GetNoiseFactor();
		if (_consumptionDelay > TimeSpan.Zero)
		{
			// Don't begin consuming the tile until the delay has elapsed.
			return;
		}

		if (_consumptionOffset == tiles.TileHeight)
		{
			// Tile consumption is complete.
			return;
		}

		_elapsedConsumption += gameTime.ElapsedTime * GetNoiseFactor();
		if (_elapsedConsumption > _consumptionRate)
		{
			_elapsedConsumption = TimeSpan.Zero;
			_consumptionOffset++;

			if (_consumptionOffset < tiles.TileHeight)
			{
				ConsumeTileRow(tiles, fountain);
			}
			else
			{
				fountain.IsActive = false;
			}
		}
	}

	private void ConsumeTileRow(GlyphSet<Bitmap> tiles, ParticleFountain fountain)
	{
		var x = (int)_position.X;
		var y = (int)_position.Y;
		var tile = tiles?[_tileIndex];
		if (tile == null) return;

		var dy = _consumptionOffset;
		for (var dx = 0; dx < tile.Width; dx++)
		{
			var value = tile.GetPixel(dx, dy);
			if (value)
			{
				fountain.IsActive = true;
				fountain.MoveTo(new Vector2(x + dx, y + dy));
			}
		}
	}

	private float GetNoiseFactor()
	{
		return Random.Shared.NextSingle() * DEFAULT_NOISE_FACTOR + (1.0f - DEFAULT_NOISE_FACTOR);
	}

	#endregion
}
