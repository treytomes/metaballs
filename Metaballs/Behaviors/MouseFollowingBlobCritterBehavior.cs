using Metaballs.Behaviors.Props;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RetroTK;

namespace Metaballs.Behaviors;

class MouseFollowingBlobCritterBehavior : BlobCritterBehavior
{
	#region Fields

	private MouseFollowingBlobCritterBehaviorProps _props;
	private Vector2? _trackingPosition = null;

	#endregion

	#region Constructors

	public MouseFollowingBlobCritterBehavior(BlobCritter owner, MouseFollowingBlobCritterBehaviorProps props)
		: base(owner)
	{
		_props = props ?? throw new ArgumentNullException(nameof(props));
	}

	#endregion

	#region Methods

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		if (!_trackingPosition.HasValue)
			return;

		float dt = (float)gameTime.ElapsedTime.TotalSeconds;

		// --- Compute desired velocity. ---
		var toMouse = _trackingPosition.Value - Owner.Position;
		float dist = toMouse.Length;

		if (dist < 1f)
			return;

		var direction = toMouse / dist;

		// Speed proportional to distance, but capped.
		var targetSpeed = MathF.Min(dist * 4f, _props.MaxSpeed);

		var targetVelocity = direction * targetSpeed;

		// --- Smooth velocity change. ---
		// dV/dt = (targetVelocity - currentVelocity) * RESPONSIVENESS
		var desiredDelta = (targetVelocity - Owner.Velocity) * _props.Responsiveness;

		// Clamp acceleration magnitude.
		var desiredAccelLength = desiredDelta.Length;
		if (desiredAccelLength > _props.MaxAcceleration)
			desiredDelta = desiredDelta / desiredAccelLength * _props.MaxAcceleration;

		Owner.Velocity += desiredDelta * dt;

		// Update critter speed property for outside use (optional).
		Owner.Speed = Owner.Velocity.Length * _props.SpeedDamping;
	}

	public override bool MouseMove(MouseMoveEventArgs e)
	{
		_trackingPosition = e.Position;
		return true;
	}

	#endregion
}
