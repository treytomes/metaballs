using Metaballs.Props;
using Metaballs.Renderables;
using RetroTK;
using RetroTK.Gfx;
using RetroTK.Services;
using RetroTK.States;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

using MouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;

namespace Metaballs.States;

class BlobCritterState : GameState
{
	#region Fields

	private readonly MetaballsAppSettings _settings;
	// private bool _isMouseDown = false;
	private Vector2 _mousePosition = Vector2.Zero;

	private BlobFactory _blobFactory;
	private List<BlobCritter> _critters = new();
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
		_critters.Add(new BlobCritter(_settings, new()
		{
			OutlineColor = new RadialColor(5, 0, 5),
			FillColor = new RadialColor(0, 5, 0),
		}, new Vector2(250, 200), new CreateBlobCritterProps()));
		_critters.Add(new BlobCritter(_settings, new()
		{
			OutlineColor = new RadialColor(5, 0, 5),
			FillColor = new RadialColor(5, 5, 0),
		}, new Vector2(150, 150), new CreateBlobCritterProps()));

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

		// Note: Running this in parallel led to a racing condition.
		// Parallel.ForEach(_critters, c => c.Render(RC));
		foreach (var c in _critters)
		{
			c.Render(RC);
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

		Parallel.ForEach(_critters, c => c.Update(gameTime));
	}

	public override bool MouseWheel(MouseWheelEventArgs e)
	{
		foreach (var c in _critters)
		{
			if (c.MouseWheel(e))
			{
				return true;
			}
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

		foreach (var c in _critters)
		{
			if (c.MouseMove(e))
			{
				return true;
			}
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
		foreach (var c in _critters)
		{
			if (c.MouseDown(e))
			{
				return true;
			}
		}

		// TODO: Implement `Contains` on blob critter.
		// var mouseHover = _critters.First(c => c.Contains(_mousePosition));
		// if (e.Button == MouseButton.Left)
		// {
		// 	if (_blobFactory == null) return false;
		// 	var blob = _blobFactory.CreateRandomBlob(new System.Drawing.Rectangle(0, 0, RC.Width, RC.Height));
		// 	blob.MoveTo(_mousePosition);
		// 	mouseHover.Add(new EventBlob(blob));
		// 	return true;
		// }

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

		foreach (var c in _critters)
		{
			if (c.MouseUp(e))
			{
				return true;
			}
		}

		return false;
	}

	#endregion
}