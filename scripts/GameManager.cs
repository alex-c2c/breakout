using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public sealed class GameManager
{
	private static readonly GameManager _instance = new GameManager();
	public static GameManager Instance
	{
		get
		{
			return _instance;
		}
	}

	private static Dictionary<int, List<string>> _levelData = null;
	private static Dictionary<int, int> _highScoreDict;
	
    public static bool IsPaused
    {
        get;
        set;
    } = false;

	public static int Lives
	{
		get;
		set;
	} = Constants.DEFAULT_LIVES;

    public static int CurrentLevel
    {
        get;
        set;
    } = 1;

	static GameManager()
	{
        
	}
	
	private GameManager()
	{
        _highScoreDict = new Dictionary<int, int>();
        Debug.Print($"GameManager");
	}

	public static bool LoadLevelData()
    {
        string dataFilePath = Godot.ProjectSettings.GlobalizePath($"res://{Constants.LEVEL_DATA_FILE}");

        Debug.Print($"Attempting to load level data file: {dataFilePath}");

        if (!File.Exists(dataFilePath))
        {
            Debug.Print($"Error: Level data file {dataFilePath} does not exists!");
            return false;
        }

        try
        {
            using (StreamReader sr = new StreamReader(dataFilePath))
            {
                string jsonString = sr.ReadToEnd();

                _levelData = JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(jsonString);

                Debug.Print("Data loaded successfully");

                return true;
            }
        }
        catch (Exception e)
        {
            Debug.Print($"Error occured while trying to load level data file");
            Debug.Print($"{e}");

            return false;
        }
    }

    public static List<string> GetCurrentLevelData()
    {
        return GetLevelData(CurrentLevel);
    }

    public static List<string> GetLevelData(int level)
    {
        _levelData.TryGetValue(level, out List<string> value);
        
        if (value.Count == 0 || value == null)
        {
            return null;
        }

        return value;
    }

	public static async Task<bool> SaveHighscoreData()
    {
        string filePath = Path.Combine(OS.GetUserDataDir(), Constants.HIGHSCORE_FILE);

        Debug.Print($"Attempting to save data to file: {filePath}");

        string jsonString = JsonConvert.SerializeObject(_highScoreDict, Formatting.Indented);

        try
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                await sw.WriteAsync(jsonString);
                Debug.Print($"Data saved successfully");
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.Print($"Error occured while trying to save file");
            Debug.Print($"Error: {e}");

            return false;
        }        
    }

    public static async Task<bool> LoadHighscoreData()
    {
        string filePath = Path.Combine(OS.GetUserDataDir(), Constants.HIGHSCORE_FILE);

        Debug.Print($"Attempting to load save file: {filePath}");

        if (!File.Exists(filePath))
        {
            SetDefaultHighscore();
            await SaveHighscoreData();
            
            Debug.Print($"Warning: Save file {filePath} does not exist!");
            Debug.Print($"Creating highscore file with default values");
            
            return true;
        }

        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string jsonString = sr.ReadToEnd();

                _highScoreDict = JsonConvert.DeserializeObject<Dictionary<int, int>>(jsonString);
            }

            Debug.Print("Data loaded successfully");

            return true;
        }
        catch (Exception e)
        {
            Debug.Print($"Error occured while trying to load file");
            Debug.Print($"Error: {e}");

            return false;
        }
    }

    private static void SetDefaultHighscore()
    {
        SetHighscore(1, 0);
        SetHighscore(2, 0);
        SetHighscore(3, 0);
    }

	public static async void SetHighscore(int level, int newHighscore)
	{
		bool success = _highScoreDict.TryGetValue(level, out int highscore);
		if (!success || newHighscore > highscore)
		{
			_highScoreDict[level] = newHighscore;
			await SaveHighscoreData();
		}
	}

	public static int GetHighscore(int level)
	{
		if (_highScoreDict.TryGetValue(level, out int highscore))
		{
			return highscore;
		}
		
		return 0;
	}
}