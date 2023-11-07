using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

public sealed class GameUtils
{
	private static readonly GameUtils _instance = new GameUtils();
	public static GameUtils Instance
	{
		get
		{
			return _instance;
		}
	}

	static GameUtils()
	{

	}
	
	private GameUtils()
	{

	}
}