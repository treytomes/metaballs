using Metaballs.Particles;
using OpenTK.Mathematics;
using RetroTK;
using RetroTK.Gfx;

namespace Metaballs;

class ConsumableTileSet
{
	#region Fields

	private readonly List<ConsumableTile> _tiles = new();
	private readonly ParticleFountain _fountain;

	#endregion

	#region Constructors

	public ConsumableTileSet(ParticleFountainFactory fountainFactory)
	{
		_fountain = fountainFactory.CreateTileBurningFountain();
	}

	#endregion

	#region Methods

	public void Render(IRenderingContext rc, GlyphSet<Bitmap> tiles, MetaballsBuffer fire)
	{
		_fountain.Render(fire);
		foreach (var tile in _tiles)
		{
			tile.Render(rc, tiles);
		}
	}

	public void Update(GameTime gameTime, GlyphSet<Bitmap> tiles)
	{
		_fountain.Update(gameTime);
		foreach (var tile in _tiles)
		{
			tile.Update(gameTime, tiles, _fountain);
		}
	}

	public void WriteString(string text, Vector2 position, RadialColor fgColor, TimeSpan renderDelay)
	{
		var x = (int)position.X;
		var y = (int)position.Y;

		var totalDelay = renderDelay;
		for (var i = 0; i < text.Length; i++)
		{
			_tiles.Add(new ConsumableTile(text[i], new Vector2(x + i * 8, y), fgColor, renderDelay: totalDelay));
			totalDelay += renderDelay;
		}
	}

	#endregion
}
