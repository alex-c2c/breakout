using Godot;
using System;

public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		GameManager.LoadLevelData();
		await GameManager.LoadHighscoreData();
	}

	private void _on_button_lv_1_pressed()
	{
		AudioManager.PlaySFX(AudioManager.Sfx.Confirm, this);

		ChangeLevel(1);
	}

	private void _on_button_lv_2_pressed()
	{
		AudioManager.PlaySFX(AudioManager.Sfx.Confirm, this);

		ChangeLevel(2);
	}

	private void _on_button_lv_3_pressed()
	{
		AudioManager.PlaySFX(AudioManager.Sfx.Confirm, this);

		ChangeLevel(3);
	}

	private void _on_button_quit_pressed()
	{
		AudioManager.PlaySFX(AudioManager.Sfx.Confirm, this);

		GetTree().Quit();
	}

	private void ChangeLevel(int level)
	{
		GameManager.CurrentLevel = level;

		GetTree().ChangeSceneToFile("res://scenes/Stage.tscn");
	}
}
