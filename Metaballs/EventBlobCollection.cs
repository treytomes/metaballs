using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RetroTK.Events;

namespace Metaballs;

class EventBlobCollection : BlobCollection<EventBlob>, IEventHandler
{
	#region Fields

	private Vector2 _mousePosition = Vector2.Zero;
	private Vector2? _mouseDragStart = null;

	#endregion

	#region Constructors

	public EventBlobCollection(MetaballsSettings settings, int width, int height, IEnumerable<EventBlob>? blobs = null)
		: base(settings, width, height, blobs ?? Enumerable.Empty<EventBlob>())
	{
	}

	#endregion

	#region Properties

	public EventBlob? MouseHover { get; private set; } = null;
	public EventBlob? MouseFocus { get; private set; } = null;

	#endregion

	#region Methods

	public bool MouseMove(MouseMoveEventArgs e)
	{
		_mousePosition = e.Position;
		if (MouseFocus != null && _mouseDragStart.HasValue)
		{
			MouseFocus.MoveBy(_mousePosition - _mouseDragStart.Value);
			_mouseDragStart = _mousePosition;
		}

		if (MouseHover != null)
		{
			MouseHover.LoseMouseHover();
			MouseHover = null;
		}

		foreach (var blob in _blobs)
		{
			if (blob.Contains(e.Position))
			{
				MouseHover = blob;
				blob.AcquireMouseHover();
				return true;
			}
		}
		return false;
	}

	public bool MouseDown(MouseButtonEventArgs e)
	{
		MouseFocus?.LoseMouseFocus();
		if (MouseHover != null)
		{
			MouseFocus = MouseHover;
			MouseFocus?.AcquireMouseFocus();
			_mouseDragStart = _mousePosition;
			return true;
		}
		return false;
	}

	public bool MouseUp(MouseButtonEventArgs e)
	{
		_mouseDragStart = null;
		if (MouseFocus != null)
		{
			MouseFocus.Velocity = Vector2.Zero;
			MouseFocus.LoseMouseFocus();

			if (e.Button == MouseButton.Right)
			{
				_blobs.Remove(MouseFocus);
			}

			MouseFocus = null;
			return true;
		}
		return false;
	}

	public bool MouseWheel(MouseWheelEventArgs e)
	{
		if (MouseHover == null)
		{
			return false;
		}
		MouseHover.SetRadius(MouseHover.Radius + Math.Sign(e.OffsetY));
		return true;
	}

	public bool KeyDown(KeyboardKeyEventArgs e)
	{
		return false;
	}

	public bool KeyUp(KeyboardKeyEventArgs e)
	{
		return false;
	}

	public bool TextInput(TextInputEventArgs e)
	{
		return false;
	}

	#endregion
}
