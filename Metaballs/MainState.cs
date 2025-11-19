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

	private EventBlobCollection _blobs = new();

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

		var factory = new BlobFactory(RC);

		for (var n = 0; n < MetaballsConfig.NumBlobs; n++)
		{
			_blobs.Add(new EventBlob(factory.CreateRandomBlob()));
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

		_blobs.Render(RC);

		base.Render(gameTime);
	}

	/// <summary>
	/// Updates the state.
	/// </summary>
	/// <param name="gameTime">Timing values for the current frame.</param>
	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		_blobs.Update(gameTime);
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
		if (_blobs.MouseWheel(e))
		{
			return true;
		}
		return base.MouseWheel(e);
	}

	/// <summary>
	/// Handles mouse move events.
	/// </summary>
	/// <param name="e">Mouse move event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool MouseMove(MouseMoveEventArgs e)
	{
		// _mousePosition = e.Position;
		// if (_isMouseDown)
		// {
		// }

		if (_blobs.MouseMove(e))
		{
			return true;
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
		// if (e.Button == MouseButton.Left)
		// {
		// 	_isMouseDown = true;
		// 	return true;
		// }
		if (_blobs.MouseDown(e)) return true;
		return false;
	}

	/// <summary>
	/// Handles mouse up events.
	/// </summary>
	/// <param name="e">Mouse button event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool MouseUp(MouseButtonEventArgs e)
	{
		// if (e.Button == MouseButton.Left)
		// {
		// 	_isMouseDown = false;
		// 	return true;
		// }
		if (_blobs.MouseUp(e)) return true;
		return false;
	}

	#endregion
}