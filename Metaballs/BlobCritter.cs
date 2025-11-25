using Metaballs.Behaviors;
using Metaballs.Bounds;
using Metaballs.Props;
using Metaballs.Renderables;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RetroTK;
using RetroTK.Events;
using RetroTK.Gfx;

namespace Metaballs;

// Note: A sticky blob collection will pull all of it's blobs towards the center of mass.
// Each blob will need a velocity.  There should also be some kind of spring constant.

class BlobCritter : IEventHandler
{
	#region Constants

	/// <summary>
	/// Cohesion keeping the spring from moving.
	/// </summary>
	private const float SPRING_STIFFNESS = 45f;

	/// <summary>
	/// Prevents jitter.
	/// </summary>
	private const float SPRING_DAMPING = 10f;
	private const float MASS_SCALAR = 1f;

	/// <summary>
	/// Helps the goo stay cohesive.
	/// </summary>
	private const float CENTER_PULL = 1.2f;

	private const bool SHOW_TARGET = false;

	#endregion

	#region Fields

	private readonly EventBlobCollection _blobs;
	private readonly List<BlobCritterBehavior> _behaviors = new();
	private readonly IBoundingArea _bounds;

	private readonly AggregateRenderable _target = new();

	#endregion

	#region Constructors

	public BlobCritter(MetaballsSettings settings, int width, int height, Vector2 position, CreateBlobCritterProps props)
	{
		_blobs = new(settings, width, height);

		Speed = props.MinSpeed + Random.Shared.NextSingle() * (props.MaxSpeed - props.MinSpeed);
		Friction = props.MinFriction + Random.Shared.NextSingle() * (props.MaxFriction - props.MinFriction);

		Position = position;

		_behaviors.Add(new MouseFollowingBlobCritterBehavior(this));

		var factory = new BlobFactory(settings);
		var numBlobs = Random.Shared.Next(props.MinNumBlobs, props.MaxNumBlobs + 1);
		for (var n = 0; n < numBlobs; n++)
		{
			_blobs.Add(new EventBlob(factory.CreateRadialBlob(Position, props.BlobProps)));
		}

		var baseRadius = _blobs.Max(x => x.Radius) * 2;
		var radius = (int)(baseRadius * props.StretchScale);
		_bounds = new CircleBoundingArea(Position, radius);

		_target.Add(new Plus()
		{
			Size = 6,
			Color = RadialColor.Yellow,
		});

		_target.Add(new Circle()
		{
			Radius = radius,
			OutlineColor = RadialColor.Yellow,
		});

		_target.IsVisible = SHOW_TARGET;
	}

	#endregion

	#region Properties


	public Vector2 Position
	{
		get
		{
			return _target.Position;
		}
		private set
		{
			_target.Position = value;
		}
	}

	public Vector2 Velocity { get; set; } = Vector2.Zero;
	public float Speed { get; set; }
	public float Friction { get; set; }

	#endregion

	#region Methods

	public void Render(IRenderingContext rc)
	{
		_blobs.Render(rc);
		_target.Render(rc);
	}

	public void Update(GameTime gameTime)
	{
		var dt = (float)gameTime.ElapsedTime.TotalSeconds;

		// --- Apply spring forces to each blob in parallel. ---
		Parallel.ForEach(_blobs, blob =>
		{
			var mass = blob.Radius * MASS_SCALAR;

			// Desired "rest" position of the blob.
			var toBlob = blob.Position - Position;
			var dist = toBlob.Length;

			Vector2 direction;
			if (dist > 0f)
				direction = toBlob / dist;
			else
				direction = Vector2.UnitX; // Arbitrary but safe fallback.

			// Desired distance from center.
			var targetDist = blob.Radius * 0.5f;

			// Signed displacement.
			var displacement = dist - targetDist;

			// Hooke’s law spring force.
			var springForceMag = SPRING_STIFFNESS * displacement;

			// Velocity along spring axis (for damping).
			var radialVel = Vector2.Dot(blob.Velocity - Velocity, direction);
			var dampingForceMag = -SPRING_DAMPING * radialVel;

			// Combined force (scalar).
			var totalForceMag = springForceMag + dampingForceMag;

			// Convert scalar → vector.
			Vector2 force = direction * totalForceMag;

			// Apply acceleration.
			blob.Velocity += force / mass * dt;
		});

		// --- Update blob positions + clamp to bounding area. ---
		Position += Velocity * Speed * dt;
		_bounds.MoveTo(Position);
		_blobs.Update(gameTime, _bounds);

		// --- Optional: pull center toward the blob cluster. ---
		// This prevents the center from drifting and makes the critter act cohesive.
		Vector2 com = _blobs.CenterOfMass;
		Vector2 comOffset = com - Position;

		if (comOffset.LengthSquared > 0.0001f)
		{
			Vector2 comDir = comOffset.Normalized();
			Velocity += comDir * CENTER_PULL * dt;
		}

		// --- Apply critter-wide friction. ---
		Velocity *= 1f - Friction * dt;

		// Update behaviors (mouse chasing, wandering, etc.).
		foreach (var behavior in _behaviors)
		{
			behavior.Update(gameTime);
		}
	}

	public void Add(EventBlob blob)
	{
		_blobs.Add(blob);
	}

	public void Remove(EventBlob blob)
	{
		_blobs.Remove(blob);
	}

	public bool MouseMove(MouseMoveEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.MouseMove(e);
		}

		return _blobs.MouseMove(e);
	}

	public bool MouseDown(MouseButtonEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.MouseDown(e);
		}

		return _blobs.MouseDown(e);
	}

	public bool MouseUp(MouseButtonEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.MouseUp(e);
		}

		return _blobs.MouseUp(e);
	}

	public bool MouseWheel(MouseWheelEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.MouseWheel(e);
		}

		return _blobs.MouseWheel(e);
	}

	public bool KeyDown(KeyboardKeyEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.KeyDown(e);
		}

		return _blobs.KeyDown(e);
	}

	public bool KeyUp(KeyboardKeyEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.KeyUp(e);
		}

		return _blobs.KeyUp(e);
	}

	public bool TextInput(TextInputEventArgs e)
	{
		foreach (var behavior in _behaviors)
		{
			behavior.TextInput(e);
		}

		return _blobs.TextInput(e);
	}

	#endregion
}