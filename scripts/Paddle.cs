using Godot;
using System;

public partial class Paddle : Area2D
{
	[Export]
	private Ball _ball;


	private const int _moveSpeed = 1000;
	private float _halfWidth;
	private double _height;
	private Vector2 _initialPosition;
	private Vector2 _initialScale;

	public bool AutoPlay
	{ 
		get;
		set;
	} = false;

	public double Height
	{
		get
		{
			return _height;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_initialPosition = this.Position;
		_initialScale = this.Scale;

		ColorRect colorRect = GetNode("ColorRect") as ColorRect;
		_halfWidth = colorRect.Size.X * 0.5f;
		_height = colorRect.Size.Y;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GameManager.IsPaused)
		{
			return;
		}

		float input = Input.GetActionStrength("ui_left") - Input.GetActionStrength("ui_right");

		Vector2 position = this.Position;
		position -= new Vector2((float)(input * _moveSpeed * delta), 0f);
		position.X = Mathf.Clamp(position.X, Constants.GAMEAREA_START_X + _halfWidth, Constants.GAMEAREA_START_X + Constants.GAMEAREA_WIDTH - _halfWidth);
		this.Position = position;

		if (!_ball.IsLaunched)
		{
			_ball.MoveX(position.X);
		}
	}

	public void Reset()
	{
		this.Position = _initialPosition;
		this.Scale = _initialScale;
	}

	public void ReduceWidth()
	{
		this.Scale = new Vector2(this.Scale.X - 0.15f, this.Scale.Y);
	}

	public Vector2 GetTopCenterPosition()
	{
		return this.Position - new Vector2(0f, 15f);
	}
}
