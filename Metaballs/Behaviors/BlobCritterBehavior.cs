using OpenTK.Windowing.Common;
using RetroTK;

namespace Metaballs.Behaviors;

abstract class BlobCritterBehavior(BlobCritter owner) : IBlobCritterBehavior
{
	public BlobCritter Owner { get; } = owner;

	public virtual void Update(GameTime gameTime) { }
	public virtual bool KeyDown(KeyboardKeyEventArgs e) => false;
	public virtual bool KeyUp(KeyboardKeyEventArgs e) => false;
	public virtual bool MouseDown(MouseButtonEventArgs e) => false;
	public virtual bool MouseMove(MouseMoveEventArgs e) => false;
	public virtual bool MouseUp(MouseButtonEventArgs e) => false;
	public virtual bool MouseWheel(MouseWheelEventArgs e) => false;
	public virtual bool TextInput(TextInputEventArgs e) => false;
}
