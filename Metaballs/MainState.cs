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

class MainState : GameState
{
	#region Fields

	private bool _isMouseDown = false;
	private Vector2 _mousePosition = Vector2.Zero;

	private List<List<float?>> _samples = new();
	private List<Blob> _blobs = new();

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="TileMapTestState"/> class.
	/// </summary>
	/// <param name="resources">Resource manager for loading assets.</param>
	/// <param name="rc">Rendering context for drawing.</param>
	public MainState(IResourceManager resources, IRenderingContext rc)
		: base(resources, rc)
	{
	}

	#endregion

	#region Methods

	/// <summary>
	/// Loads resources and initializes the state.
	/// </summary>
	public override void Load()
	{
		base.Load();

		for (var n = 0; n < MetaballsConfig.NumBlobs; n++)
		{
			_blobs.Add(Blob.CreateRandom(RC));
		}
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

		// Draw the grid.
		if (MetaballsConfig.ShowGrid)
		{
			for (var x = 0; x < RC.Width; x += MetaballsConfig.GridResolution)
			{
				RC.RenderLine(new Vector2(x, 0), new Vector2(x, RC.Height - 1), RadialColor.Gray);
			}

			for (var y = 0; y < RC.Height; y += MetaballsConfig.GridResolution)
			{
				RC.RenderLine(new Vector2(0, y), new Vector2(RC.Width - 1, y), RadialColor.Gray);
			}
		}


		// Render the line cases.
		for (var sy = 0; sy < _samples.Count - 2; sy++)
		{
			for (var sx = 0; sx < _samples[sy].Count - 2; sx++)
			{
				// if PRINT_SAMPLES then
				// 	x = sx * GRID_RESOLUTION
				// 	y = sy * GRID_RESOLUTION
				// 	sample = calculateSample(x, y)
				// 	sample = floor(sample * 100) / 100
				// 	gfx.print sample, x -16, y - 16, color.silver, "small"
				// end if

				var bl = CalculateSample(sx, sy) ?? 0;
				var br = CalculateSample(sx + 1, sy) ?? 0;
				var tl = CalculateSample(sx, sy + 1) ?? 0;
				var tr = CalculateSample(sx + 1, sy + 1) ?? 0;

				Vector2 a, b, c, d;

				if (MetaballsConfig.Interpolated)
				{
					a = new Vector2(
						sx * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution * Lerp(1, tl, tr),
						sy * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution
					);
					b = new Vector2(
						sx * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution,
						sy * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution * Lerp(1, br, tr)
					);
					c = new Vector2(
						sx * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution * Lerp(1, bl, br),
						sy * MetaballsConfig.GridResolution
					);
					d = new Vector2(
						sx * MetaballsConfig.GridResolution,
						sy * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution * Lerp(1, bl, tl)
					);
				}
				else
				{
					a = new Vector2(
						sx * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution / 2,
						sy * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution
					);
					b = new Vector2(
						sx * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution,
						sy * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution / 2
					);
					c = new Vector2(
						sx * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution / 2,
						sy * MetaballsConfig.GridResolution
					);
					d = new Vector2(
						sx * MetaballsConfig.GridResolution,
						sy * MetaballsConfig.GridResolution + MetaballsConfig.GridResolution / 2
					);
				}

				bl = bl >= 1 ? 1 : 0;
				br = br >= 1 ? 1 : 0;
				tl = tl >= 1 ? 1 : 0;
				tr = tr >= 1 ? 1 : 0;
				var blobCase = bl + br * 2 + tr * 4 + tl * 8;

				if (blobCase == 0 || blobCase == 15)
					continue;
				else if (blobCase == 1 || blobCase == 14)
					RenderLine(d, c);
				else if (blobCase == 2 || blobCase == 13)
					RenderLine(b, c);
				else if (blobCase == 3 || blobCase == 12)
					RenderLine(d, b);
				else if (blobCase == 4 || blobCase == 11)
					RenderLine(a, b);
				else if (blobCase == 5)
				{
					RenderLine(d, a);
					RenderLine(c, b);
				}
				else if (blobCase == 6 || blobCase == 9)
					RenderLine(c, a);
				else if (blobCase == 7 || blobCase == 8)
					RenderLine(d, a);
				else if (blobCase == 10)
				{
					RenderLine(a, b);
					RenderLine(c, d);
				}
			}
		}

		foreach (var blob in _blobs)
		{
			blob.Render(RC);
		}

		base.Render(gameTime);
	}

	/// <summary>
	/// Updates the state.
	/// </summary>
	/// <param name="gameTime">Timing values for the current frame.</param>
	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		ResetSamples();

		foreach (var blob in _blobs)
		{
			blob.Update(gameTime);
		}
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
		return base.MouseWheel(e);
	}

	/// <summary>
	/// Handles mouse move events.
	/// </summary>
	/// <param name="e">Mouse move event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool MouseMove(MouseMoveEventArgs e)
	{
		_mousePosition = e.Position;
		if (_isMouseDown)
		{
		}
		return base.MouseMove(e);
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
			_isMouseDown = true;
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
			_isMouseDown = false;
			return true;
		}
		return false;
	}

	private float Lerp(float x, float x0, float x1, float y0 = 0, float y1 = 1)
	{
		if (x0 == x1)
		{
			return 0.0f;
		}
		return y0 + ((y1 - y0) * (x - x0)) / (x1 - x0);
	}

	private void ResetSamples()
	{
		_samples = new List<List<float?>>();

		for (var y = 0; y < RC.Height; y += MetaballsConfig.GridResolution)
		{
			var row = new List<float?>();
			for (var x = 0; x < RC.Width; x += MetaballsConfig.GridResolution)
			{
				row.Add(null);
			}
			_samples.Add(row);
		}
	}

	private float? CalculateSample(int sx, int sy)
	{
		if (_samples[sy][sx] != null) return _samples[sy][sx];
		var x = sx * MetaballsConfig.GridResolution;
		var y = sy * MetaballsConfig.GridResolution;

		var sample = 0f;
		foreach (var b in _blobs)
		{
			sample += (float)Math.Pow(b.Radius, 2) / ((float)Math.Pow(x - b.Position.X, 2) + (float)Math.Pow(y - b.Position.Y, 2));
		}
		_samples[sy][sx] = sample;
		return sample;
	}

	private void RenderLine(Vector2 from, Vector2 to)
	{
		RC.RenderLine(from, to, RadialColor.Green);
	}

	#endregion
}