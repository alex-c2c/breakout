using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class Stage : Node2D
{
	

	[Export]
	private Paddle _paddle;

	[Export]
	private Ball _ball;

	[Export]
	private Node2D _targets;

	[Export]
	private Node2D _menu;

	[Export]
	private Label _labelLevel;

	[Export]
	private Label _labelScore;

	[Export]
	private Label _labelHighscore;

	[Export]
	private Sprite2D[] _lifeArray;

	private int _currentLevel = 0;
	private int _score = 0;
	private int _highestTargetPoint = 0;

	public int Lives
	{
		get;
		set;
	} = Constants.DEFAULT_LIVES;

	private int Level
	{
		get;
		set;
	} = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_currentLevel = GameManager.CurrentLevel;

		SetupLevel();

		_labelLevel.Text = _currentLevel.ToString();
		_labelScore.Text = _score.ToString();
		_labelHighscore.Text = GameManager.GetHighscore(_currentLevel).ToString();

		RefreshLifeIcons();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_text_clear_carets_and_selection"))
		{
			AudioManager.PlaySFX(AudioManager.Sfx.Cancel, this);

			ToggleMenu();
		}
		else if (Input.IsActionJustPressed("ui_accept"))
		{
			if (!_ball.IsLaunched)
			{
				_ball.IsLaunched = true;
				
				AudioManager.PlaySFX(AudioManager.Sfx.Select, this);
			}
		}
	}

	private void ClearLevel()
	{
		foreach (var child in _targets.GetChildren())
		{
			child.QueueFree();
		}
	}

	private void SetupLevel()
	{
		List<string> levelData = GameManager.GetCurrentLevelData();
		
		PackedScene targetRes = ResourceLoader.Load(Godot.ProjectSettings.GlobalizePath("res://scenes/Target.tscn")) as PackedScene;
		float startX = Constants.GAMEAREA_START_X;
		float startY = Constants.GAMEAREA_START_Y;

		foreach (string lineData in levelData)
		{
			List<int> lineList = lineData.Split(',')?.Select(Int32.Parse)?.ToList();

			int count = lineList.Count;
			float targetWidth = Constants.GAMEAREA_WIDTH / count;
			float scaleX = targetWidth / 10f; // target starting width is 10f
			float scaleY = 1.5f; // target starting heigth is 10f

			foreach (int data in lineList)
			{
				Target newTarget = targetRes.Instantiate() as Target;
				newTarget.SetData(startX, startY, scaleX, scaleY, data, data);
				_targets.AddChild(newTarget);

				startX += targetWidth;
			}

			startX = Constants.GAMEAREA_START_X;
			startY += 20f;
		}
	}

	private void ToggleMenu()
	{
		_menu.Visible = !GameManager.IsPaused;
		GameManager.IsPaused = _menu.Visible;
	}

	private void _on_button_reset_pressed()
	{
		AudioManager.PlaySFX(AudioManager.Sfx.Confirm, this);

		ResetGame();

		ToggleMenu();
	}

	private void _on_button_back_pressed()
	{
		AudioManager.PlaySFX(AudioManager.Sfx.Confirm, this);

		GetTree().ChangeSceneToFile("res://scenes/Main.tscn");
	}

	private void _on_button_quit_pressed()
	{
		AudioManager.PlaySFX(AudioManager.Sfx.Confirm, this);

		GetTree().Quit();
	}

	public void RefreshLifeIcons()
	{
		for (int i = 0; i < _lifeArray.Count(); i++)
		{
			var icon = _lifeArray[i];
			icon.Visible = i < Lives;
		}
	}

	public void IncreaseScore(int points)
	{
		if (points > _highestTargetPoint)
		{
			_highestTargetPoint = points;

			if (_highestTargetPoint >= 2)
			{
				_paddle.ReduceWidth();
			}
		}

		_score += points;
		_labelScore.Text = _score.ToString();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns>True if Life > 0, else False</returns>
	public bool ReduceLife()
	{
		Lives -= 1;

		if (Lives <= 0)
		{
			ResetGame();

			return false;
		}
		else
		{
			RefreshLifeIcons();

			return true;
		}
	}

	private void ResetGame()
	{
		GameManager.SetHighscore(_currentLevel, _score);
			
		int newHighscore = GameManager.GetHighscore(_currentLevel);
		_labelHighscore.Text = newHighscore.ToString();

		_score = 0;
		_labelScore.Text = _score.ToString();

		_paddle.Reset();
		_ball.Reset();

		Lives = Constants.DEFAULT_LIVES;
		RefreshLifeIcons();

		ClearLevel();

		CallDeferred("SetupLevel");
	}
}
