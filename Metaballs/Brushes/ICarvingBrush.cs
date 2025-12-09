using OpenTK.Mathematics;
using RetroTK;
using RetroTK.Gfx;

namespace Metaballs.Brushes;

interface ICarvingBrush
{
	/// <summary>
	/// Render a hint as to the what and where of this brush.
	/// </summary>
	void Render(IRenderingContext rc, Vector2 position);

	void Carve(GameTime gameTime, SampleMap samples, Vector2 position);
}