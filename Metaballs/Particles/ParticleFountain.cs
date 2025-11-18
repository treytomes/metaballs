using OpenTK.Mathematics;
using RetroTK;

namespace Metaballs.Particles;

class ParticleFountain
{
	#region Fields

	private readonly ParticleFountainProps _props;
	private readonly List<Particle> _particles = new();
	private TimeSpan _spawnTimer = TimeSpan.Zero;

	#endregion

	#region Constructors

	public ParticleFountain(Vector2 position, ParticleFountainProps props)
	{
		_props = props;
		Position = position;
		_spawnTimer = TimeSpan.Zero;
	}

	#endregion

	#region Properties

	public Vector2 Position { get; private set; }
	public bool IsActive { get; set; } = false;
	public bool IsVisible { get; set; } = true;

	public float Scale
	{
		get
		{
			return _props.Scale;
		}
		set
		{
			if (value < 0) value = 0;
			_props.Scale = value;
		}
	}

	#endregion

	#region Methods

	// Note: Particles need to be spawned right away when the fountain is moved, otherwise several moves might happen before the next
	// spawning, which leads to weird jumpy visual effects.
	public void MoveTo(Vector2 position)
	{
		Position = position;
		_spawnTimer = TimeSpan.Zero;

		// Note: Spawn the initial particles, otherwise weird timing effects when drawing with the mouse.
		SpawnParticle();
	}

	public void Update(GameTime gameTime)
	{
		// Note: Only spawn particles while active, but you need to keep updating the existing particles to avoid weird immortal-particle effects.
		if (IsActive)
		{
			_spawnTimer += gameTime.ElapsedTime;
			if (_spawnTimer >= _props.SpawnRate)
			{
				SpawnParticle();
				_spawnTimer = TimeSpan.Zero;
			}
		}

		var deadParticles = new List<Particle>();
		for (var n = 0; n < _particles.Count; n++)
		{
			_particles[n].Update(gameTime);
			if (!_particles[n].IsAlive)
			{
				deadParticles.Add(_particles[n]);
			}
		}
		foreach (var particle in deadParticles)
		{
			_particles.Remove(particle);
		}
	}

	public void Render(FireBuffer buffer)
	{
		if (!IsVisible) return;

		foreach (var particle in _particles)
		{
			if (particle.IsAlive)
			{
				particle.Render(buffer);
			}
		}
	}

	private void SpawnParticle()
	{
		var position = Position;

		var velocity = _props.Velocity * (GetNoiseFactor() - 0.5f);
		var acceleration = _props.Acceleration * (GetNoiseFactor() - 0.5f);
		var lifeSpan = _props.LifeSpan * GetNoiseFactor();
		var scale = _props.Scale * GetNoiseFactor();

		_particles.Add(new Particle(position, velocity, acceleration, lifeSpan, scale, _props.Brush));
	}

	private float GetNoiseFactor()
	{
		return Random.Shared.NextSingle() * _props.Noise + (1.0f - _props.Noise);
	}

	#endregion
}
