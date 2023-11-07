using Godot;
using System;
using System.Diagnostics;

public partial class Target : Area2D
{
	[Export]
	private ColorRect _colorRect;

	public Vector2 Size
	{
		get;
		set;
	}

	public int Points
	{
		get;
		set;
	} = 1;

	public int Hp
	{
		get;
		set;
	} = 1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetData(float x, float y, float scaleX, float scaleY, int points, int hp)
	{
		Points = points;
		Hp = hp;

		switch (Hp)
		{
			case 5:
			{
				_colorRect.Color = Color.Color8(255, 0, 0);
			}
			break;
			case 4:
			{
				_colorRect.Color = Color.Color8(255, 128, 0);
			}
			break;
			case 3:
			{
				_colorRect.Color = Color.Color8(255, 255, 0);
			}
			break;
			case 2:
			{
				_colorRect.Color = Color.Color8(0, 255, 0);
			}
			break;
			case 1:
			{
				_colorRect.Color = Color.Color8(0, 0, 255);
			}
			break;
			default:
			{
				_colorRect.Color = Color.Color8(255, 255, 255);
			}
			break;
		}

		this.Position = new Vector2(x, y);
		this.Scale = new Vector2(scaleX, scaleY);
	}
}
