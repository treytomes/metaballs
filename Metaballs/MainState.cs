using RetroTK;
using RetroTK.Gfx;
using RetroTK.Services;
using RetroTK.States;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

using MouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;
using Metaballs.Brushes;

namespace Metaballs;

// Metaballs / Marching Squares Demo
// https://jamie-wong.com/2014/08/19/metaballs-and-marching-squares/
// https://jurasic.dev/marching_squares/

// Based on my MiniScript code here: https://gist.github.com/treytomes/3f15d6e93b2448d05a1c1e5fc0125225

// Note: It has occurred to me that if I am editing the metaball sample field

// Note: Drawing with the carving brush is surprisingly satisfying.

/// <summary>
/// Left click to draw, right click to erase, mouse wheel to control the size of the brush.
/// </summary>
class MainState : GameState
{
	#region Fields

	private readonly MetaballsAppSettings _settings;
	private Vector2 _mousePosition = Vector2.Zero;

	private CircleCarvingBrush _brush = new CircleCarvingBrush();
	private SampleMap _samples;
	private bool _isDrawing = false;

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
		_samples = new(_settings.Metaballs, rc.Width, rc.Height);
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

		RenderGrid();

		_brush.Render(RC, _mousePosition);

		_samples.Render(RC);

		base.Render(gameTime);
	}

	private void RenderGrid()
	{
		var gridResolution = 8;
		for (var x = 0; x < RC.Width; x += gridResolution)
		{
			RC.RenderLine(new Vector2(x, 0), new Vector2(x, RC.Height - 1), RadialColor.Gray);
		}
		for (var y = 0; y < RC.Height; y += gridResolution)
		{
			RC.RenderLine(new Vector2(0, y), new Vector2(RC.Width - 1, y), RadialColor.Gray);
		}
	}

	/// <summary>
	/// Updates the state.
	/// </summary>
	/// <param name="gameTime">Timing values for the current frame.</param>
	public override void Update(GameTime gameTime)
	{
		if (_isDrawing)
		{
			_brush.Carve(_samples, _mousePosition);
		}
		base.Update(gameTime);
	}

	public override bool MouseWheel(MouseWheelEventArgs e)
	{
		_brush.Radius += Math.Sign(e.OffsetY);
		return base.MouseWheel(e);
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
			_isDrawing = true;
			_brush.Intensity = Math.Abs(_brush.Intensity);
			return true;
		}
		else if (e.Button == MouseButton.Right)
		{
			_isDrawing = true;
			_brush.Intensity = -Math.Abs(_brush.Intensity);
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
			_isDrawing = false;
			return true;
		}
		else if (e.Button == MouseButton.Right)
		{
			_isDrawing = false;
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

	#endregion
}