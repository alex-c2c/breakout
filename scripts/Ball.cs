using Godot;
using System;
using System.Diagnostics;

public partial class Ball : Area2D
{
	[Export]
	private Paddle _paddle;

	[Export]
	private Stage _stage;

	private const float DEFAULT_SPEED = 600.0f;
	private const float BOUNCE_FACTOR_X = 0.2f;

	private double _speed = DEFAULT_SPEED;
	private Vector2 _size;
	public Vector2 _direction = Vector2.Up;

	public bool IsLaunched { get; set; } = false;

	


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ColorRect rect = GetNode("ColorRect") as ColorRect;
		_size = rect.Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!IsLaunched || GameManager.IsPaused)
		{
			return;
		}

		//_speed += delta * 2;
		Vector2 v = new Vector2((float)(_direction.X * _speed * delta), (float)(_direction.Y * _speed * delta));
		this.Position += v;
	}

	public void Reset()
	{
		IsLaunched = false;

		_direction = Vector2.Up;

		this.Position = _paddle.GetTopCenterPosition();

		_speed = DEFAULT_SPEED;
	}

	public void MoveX(float x)
	{
		Vector2 position = this.Position;
		this.Position = new Vector2(x, position.Y);
	}

	public Vector2 GetSize()
	{
		return _size;
	}

	private void HitPaddle()
	{
		// +ve value: right of paddle
		// -ve value: left of paddle 
		double diffX = this.Position.X - _paddle.Position.X;

		_direction = new Vector2((float)diffX * BOUNCE_FACTOR_X, (float)_paddle.Height * 0.5f * -1f).Normalized();
	}

	private void HitTarget(Target target)
	{
		// +ve value: right of paddle
		// -ve value: left of paddle 
		double diffX = this.Position.X - target.Position.X;

		_direction = new Vector2((float)diffX * BOUNCE_FACTOR_X, (float)_paddle.Height * 0.5f).Normalized();

		target.QueueFree();

		_stage.IncreaseScore(target.Points);
	}

	private void HitSideWall()
	{
		_direction = new Vector2(_direction.X * -1f, _direction.Y);
	}

	private void HitTopWall()
	{
		_direction = new Vector2(_direction.X, _direction.Y * -1f);
	}

	private void _on_area_entered(Area2D area)
	{
		string name = area.Name.ToString().ToLower();

		if (name == "wallleft" || name == "wallright")
		{
			HitSideWall();

			AudioManager.PlaySFX(AudioManager.Sfx.Select, this);
		}
		else if (name == "walltop")
		{
			HitTopWall();

			AudioManager.PlaySFX(AudioManager.Sfx.Select, this);
		}
		else if (name.Contains("bottom"))
		{
			AudioManager.PlaySFX(AudioManager.Sfx.Beep, this);

			if (_stage.ReduceLife())
			{
				Reset();
			}
		}
		else if (area is Paddle)
		{
			HitPaddle();

			AudioManager.PlaySFX(AudioManager.Sfx.Select, this);
		}
		else if (area is Target)
		{
			HitTarget(area as Target);

			_speed += 10f;

			AudioManager.PlaySFX(AudioManager.Sfx.Select, this);
		}
	}
}
