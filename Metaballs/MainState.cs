using RetroTK;
using RetroTK.Gfx;
using RetroTK.Services;
using RetroTK.States;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;
using MouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;

namespace Metaballs;

// Metaballs / Marching Squares Demo
// https://jamie-wong.com/2014/08/19/metaballs-and-marching-squares/
// https://jurasic.dev/marching_squares/

// Based on my MiniScript code here: https://gist.github.com/treytomes/3f15d6e93b2448d05a1c1e5fc0125225

// Note: It has occurred to me that if I am editing the metaball sample field

class MainState : GameState
{
	#region Constants

	private const int CARVING_RADIUS = 8;
	private const float CARVING_FACTOR = 0.1f;

	#endregion
	#region Fields

	private readonly MetaballsAppSettings _settings;
	private Vector2 _mousePosition = Vector2.Zero;

	private float[,] _samples;
	private float _drawingDelta = 0;

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="TileMapTestState"/> class.
	/// </summary>
	/// <param name="resources">Resource manager for loading assets.</param>
	/// <param name="rc">Rendering context for drawing.</param>
	public MainState(MetaballsAppSettings settings, IResourceManager resources, IRenderingContext rc)
		: base(resources, rc)
	{
		_settings = settings ?? throw new ArgumentNullException(nameof(settings));
		_samples = new float[rc.Height, rc.Width];
		Array.Clear(_samples);
	}

	#endregion

	#region Methods

	/// <summary>
	/// Loads resources and initializes the state.
	/// </summary>
	public override void Load()
	{
		base.Load();
	}

	/// <summary>
	/// Unloads resources and cleans up the state.
	/// </summary>
	public override void Unload()
	{
		base.Unload();
	}

	/// <summary>
	/// Called when this state becomes the active state.
	/// </summary>
	public override void AcquireFocus()
	{
		base.AcquireFocus();
	}

	/// <summary>
	/// Called when this state is no longer the active state.
	/// </summary>
	public override void LostFocus()
	{
		base.LostFocus();
	}

	/// <summary>
	/// Renders the state.
	/// </summary>
	/// <param name="gameTime">Timing values for the current frame.</param>
	public override void Render(GameTime gameTime)
	{
		RC.Clear();

		var gridResolution = 8;
		for (var x = 0; x < RC.Width; x += gridResolution)
		{
			RC.RenderLine(new Vector2(x, 0), new Vector2(x, RC.Height - 1), RadialColor.Gray);
		}

		for (var y = 0; y < RC.Height; y += gridResolution)
		{
			RC.RenderLine(new Vector2(0, y), new Vector2(RC.Width - 1, y), RadialColor.Gray);
		}

		RenderOutline(RC);

		base.Render(gameTime);
	}

	/// <summary>
	/// Updates the state.
	/// </summary>
	/// <param name="gameTime">Timing values for the current frame.</param>
	public override void Update(GameTime gameTime)
	{
		if (_drawingDelta != 0)
		{
			CarveChunk(_mousePosition, _drawingDelta * CARVING_FACTOR);
		}
		base.Update(gameTime);
	}

	/// <summary>
	/// Handles key down events.
	/// </summary>
	/// <param name="e">Key event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool KeyDown(KeyboardKeyEventArgs e)
	{
		return base.KeyDown(e);
	}

	public override bool KeyUp(KeyboardKeyEventArgs e)
	{
		return base.KeyUp(e);
	}

	public override bool MouseWheel(MouseWheelEventArgs e)
	{
		CarveChunk(_mousePosition, Math.Sign(e.OffsetY) * CARVING_FACTOR);
		return base.MouseWheel(e);
	}

	private void CarveChunk(Vector2 position, float delta)
	{
		for (var dy = -CARVING_RADIUS; dy <= CARVING_RADIUS; dy++)
		{
			for (var dx = -CARVING_RADIUS; dx <= CARVING_RADIUS; dx++)
			{
				var dist = (float)Math.Sqrt(dy * dy + dx * dx);
				if (dist > CARVING_RADIUS) continue;

				var sampleFactor = delta * (CARVING_RADIUS - dist);
				var x = dx + (int)position.X;
				var y = dy + (int)position.Y;
				var newSample = _samples[y, x] + sampleFactor;
				if (newSample < 0) newSample = 0;
				_samples[y, x] = newSample;
			}
		}
	}

	/// <summary>
	/// Handles mouse down events.
	/// </summary>
	/// <param name="e">Mouse button event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool MouseDown(MouseButtonEventArgs e)
	{
		if (e.Button == MouseButton.Left)
		{
			_drawingDelta = -1;
			return true;
		}
		else if (e.Button == MouseButton.Right)
		{
			_drawingDelta = 1;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Handles mouse up events.
	/// </summary>
	/// <param name="e">Mouse button event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool MouseUp(MouseButtonEventArgs e)
	{
		if (e.Button == MouseButton.Left)
		{
			_drawingDelta = 0;
			return true;
		}
		else if (e.Button == MouseButton.Right)
		{
			_drawingDelta = 0;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Handles mouse move events.
	/// </summary>
	/// <param name="e">Mouse move event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool MouseMove(MouseMoveEventArgs e)
	{
		_mousePosition = e.Position;
		return base.MouseMove(e);
	}

	// /// <summary>
	// /// Handles mouse down events.
	// /// </summary>
	// /// <param name="e">Mouse button event arguments.</param>
	// /// <returns>True if the event was handled; otherwise, false.</returns>
	// public override bool MouseDown(MouseButtonEventArgs e)
	// {
	// 	if (e.Button == MouseButton.Left)
	// 	{
	// 		// Increase sample field value.
	// 		return true;
	// 	}
	// 	else if (e.Button == MouseButton.Right)
	// 	{
	// 		// Decrease sample field value.
	// 	}

	// 	return false;
	// }

	// /// <summary>
	// /// Handles mouse up events.
	// /// </summary>
	// /// <param name="e">Mouse button event arguments.</param>
	// /// <returns>True if the event was handled; otherwise, false.</returns>
	// public override bool MouseUp(MouseButtonEventArgs e)
	// {
	// 	if (e.Button == MouseButton.Left)
	// 	{
	// 		_isMouseDown = false;
	// 		return true;
	// 	}
	// 	return false;
	// }


	private void RenderOutline(IRenderingContext rc)
	{
		Parallel.For(0, _samples.GetLength(0) - 2, sy =>
			Parallel.For(0, _samples.GetLength(1) - 2, sx =>
			{
				RenderSegments(rc, sx, sy);
			})
		);
	}

	private bool Interpolated { get; } = false;
	private float GridResolution { get; } = 1;
	private RadialColor OutlineColor { get; } = RadialColor.Yellow;

	private void RenderSegments(IRenderingContext rc, int sx, int sy)
	{
		var bl = _samples[sy, sx];
		var br = _samples[sy, sx + 1];
		var tl = _samples[sy + 1, sx];
		var tr = _samples[sy + 1, sx + 1];

		Vector2 a, b, c, d;

		// Note: If the grid resolution is 1, there's really no gain in interpolation.
		if (Interpolated && GridResolution != 1)
		{
			a = new Vector2(
				sx * GridResolution + GridResolution * Lerp(1, tl, tr),
				sy * GridResolution + GridResolution
			);
			b = new Vector2(
				sx * GridResolution + GridResolution,
				sy * GridResolution + GridResolution * Lerp(1, br, tr)
			);
			c = new Vector2(
				sx * GridResolution + GridResolution * Lerp(1, bl, br),
				sy * GridResolution
			);
			d = new Vector2(
				sx * GridResolution,
				sy * GridResolution + GridResolution * Lerp(1, bl, tl)
			);
		}
		else
		{
			a = new Vector2(
				sx * GridResolution + GridResolution / 2,
				sy * GridResolution + GridResolution
			);
			b = new Vector2(
				sx * GridResolution + GridResolution,
				sy * GridResolution + GridResolution / 2
			);
			c = new Vector2(
				sx * GridResolution + GridResolution / 2,
				sy * GridResolution
			);
			d = new Vector2(
				sx * GridResolution,
				sy * GridResolution + GridResolution / 2
			);
		}

		bl = bl >= 1 ? 1 : 0;
		br = br >= 1 ? 1 : 0;
		tl = tl >= 1 ? 1 : 0;
		tr = tr >= 1 ? 1 : 0;
		var blobCase = bl + br * 2 + tr * 4 + tl * 8;

		if (blobCase == 0 || blobCase == 15)
		{
			// skip
		}
		else if (blobCase == 1 || blobCase == 14)
		{
			RenderSegment(rc, d, c, OutlineColor);
		}
		else if (blobCase == 2 || blobCase == 13)
		{
			RenderSegment(rc, b, c, OutlineColor);
		}
		else if (blobCase == 3 || blobCase == 12)
		{
			RenderSegment(rc, d, b, OutlineColor);
		}
		else if (blobCase == 4 || blobCase == 11)
		{
			RenderSegment(rc, a, b, OutlineColor);
		}
		else if (blobCase == 5)
		{
			RenderSegment(rc, d, a, OutlineColor);
			RenderSegment(rc, c, b, OutlineColor);
		}
		else if (blobCase == 6 || blobCase == 9)
		{
			RenderSegment(rc, c, a, OutlineColor);
		}
		else if (blobCase == 7 || blobCase == 8)
		{
			RenderSegment(rc, d, a, OutlineColor);
		}
		else if (blobCase == 10)
		{
			RenderSegment(rc, a, b, OutlineColor);
			RenderSegment(rc, c, d, OutlineColor);
		}
	}

	private static void RenderSegment(IRenderingContext rc, Vector2 from, Vector2 to, RadialColor color)
	{
		if (from == to)
		{
			rc.SetPixel(from, color);
		}
		else
		{
			rc.RenderLine(from, to, color);
		}
	}

	private float Lerp(float x, float x0, float x1, float y0 = 0, float y1 = 1)
	{
		if (x0 == x1)
		{
			return 0.0f;
		}
		return y0 + ((y1 - y0) * (x - x0)) / (x1 - x0);
	}

	#endregion
}