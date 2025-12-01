namespace Metaballs.Behaviors.Props;

record MouseFollowingBlobCritterBehaviorProps : BlobCritterBehaviorProps
{
	public float MaxAcceleration = 50f;
	public float MaxSpeed = 200f;

	/// <summary>
	/// The speed with which the critter will adjust to the mouse position.
	/// </summary>
	public float Responsiveness = 0.5f;

	public float SpeedDamping = 0.05f;
}
