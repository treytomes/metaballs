using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RetroTK;

namespace Metaballs.Behaviors;

class MouseFollowingBlobCritterBehavior : BlobCritterBehavior
{
	private Vector2? _trackingPosition = null;

	public MouseFollowingBlobCritterBehavior(BlobCritter owner)
		: base(owner)
	{
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		if (!_trackingPosition.HasValue) return;

		var distance = _trackingPosition.Value - Owner.Position;
		var direction = distance.Normalized();
		Owner.Velocity = direction;
		Owner.Speed = distance.Length;
	}

	public override bool MouseMove(MouseMoveEventArgs e)
	{
		_trackingPosition = e.Position;
		return true;
	}
}
