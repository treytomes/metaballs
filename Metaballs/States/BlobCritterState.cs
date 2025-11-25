using RetroTK;
using RetroTK.Gfx;
using RetroTK.Services;
using RetroTK.States;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

using MouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;
using Metaballs.Props;
using Metaballs.Renderables;

namespace Metaballs.States;

class BlobCritterState : GameState
{
	#region Fields

	private readonly MetaballsAppSettings _settings;
	private bool _isMouseDown = false;
	private Vector2 _mousePosition = Vector2.Zero;

	private BlobFactory _blobFactory;
	private BlobCritter _blobs;
	private Grid _grid;

	#endregion

	#region Constructors

	/// <summary>
	/// Initializes a new instance of the <see cref="TileMapTestState"/> class.
	/// </summary>
	/// <param name="resources">Resource manager for loading assets.</param>
	/// <param name="rc">Rendering context for drawing.</param>
	public BlobCritterState(MetaballsAppSettings settings, IResourceManager resources, IRenderingContext rc)
		: base(resources, rc)
	{
		_settings = settings ?? throw new ArgumentNullException(nameof(settings));
		_blobFactory = new BlobFactory(_settings);
		_blobs = new BlobCritter(_settings, rc.Width, rc.Height, new Vector2(150, 150), new CreateBlobCritterProps());

		_grid = new()
		{
			Position = new Vector2(rc.Width / 2, rc.Height / 2),
			Width = rc.Width,
			Height = rc.Height,
			Resolution = 8,
			Color = RadialColor.Gray
		};
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

		// Draw the grid.
		if (_settings.Debug)
		{
			_grid.Render(RC);
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
		_mousePosition = e.Position;

		if (_blobs.MouseMove(e)) return true;

		return base.MouseMove(e);
	}

	/// <summary>
	/// Handles mouse down events.
	/// </summary>
	/// <param name="e">Mouse button event arguments.</param>
	/// <returns>True if the event was handled; otherwise, false.</returns>
	public override bool MouseDown(MouseButtonEventArgs e)
	{
		if (_blobs.MouseDown(e)) return true;

		if (e.Button == MouseButton.Left)
		{
			if (_blobFactory == null) return false;
			var blob = _blobFactory.CreateRandomBlob(new System.Drawing.Rectangle(0, 0, RC.Width, RC.Height));
			blob.MoveTo(_mousePosition);
			_blobs.Add(new EventBlob(blob));
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