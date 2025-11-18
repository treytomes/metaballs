using OpenTK.Windowing.Common;

namespace RetroTK.Events;

public interface IEventHandler
{
	bool KeyDown(KeyboardKeyEventArgs e);
	bool KeyUp(KeyboardKeyEventArgs e);
	bool MouseDown(MouseButtonEventArgs e);
	bool MouseUp(MouseButtonEventArgs e);
	bool MouseMove(MouseMoveEventArgs e);
	bool MouseWheel(MouseWheelEventArgs e);
	bool TextInput(TextInputEventArgs e);
}