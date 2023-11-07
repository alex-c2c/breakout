using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public sealed class AudioManager
{
	private static readonly AudioManager _instance = new AudioManager();
	public static AudioManager Instance
	{
		get
		{
			return _instance;
		}
	}

	public enum Sfx
	{
		Cancel = 1,
		Confirm,
		Select,
		Beep
	}

	private static Dictionary<string, AudioStreamWav> _audioDict;

	static AudioManager()
	{

	}
	
	private AudioManager()
	{
		_audioDict = new Dictionary<string, AudioStreamWav>();

		PreloadAudio();
	}

	public static void PreloadAudio()
	{
		_audioDict["cancel"] = ResourceLoader.Load(Godot.ProjectSettings.GlobalizePath("res://audio/sfx/cancel.wav")) as AudioStreamWav;
		_audioDict["confirm"] = ResourceLoader.Load(Godot.ProjectSettings.GlobalizePath("res://audio/sfx/confirm.wav")) as AudioStreamWav;
		_audioDict["select"] = ResourceLoader.Load(Godot.ProjectSettings.GlobalizePath("res://audio/sfx/select.wav")) as AudioStreamWav;
		_audioDict["beep"] = ResourceLoader.Load(Godot.ProjectSettings.GlobalizePath("res://audio/sfx/beep.wav")) as AudioStreamWav;
	}

	public static void PlaySFX(Sfx sfx, Node parent)
	{
		string sfxName = GetSfxName(sfx);
		if (string.IsNullOrEmpty(sfxName))
		{
			Debug.Print($"Error! Unable to play SFX: {sfx.ToString()}");
			return;
		}

		if (_audioDict.TryGetValue(sfxName, out AudioStreamWav wav))
		{
			AudioStreamPlayer asp = new AudioStreamPlayer();
			parent.AddChild(asp);

			asp.Stream = wav;
			asp.Play(0f);
			asp.Finished += asp.QueueFree;
		}
	}

	private static string GetSfxName(Sfx sfx)
	{
		string s = "";

		switch(sfx)
		{
			case Sfx.Cancel:
				s = "cancel";
				break;
			case Sfx.Confirm:
				s = "confirm";
				break;
			case Sfx.Select:
				s = "confirm";
				break;
			case Sfx.Beep:
				s = "beep";
				break;
		}

		return s;
	}
}
