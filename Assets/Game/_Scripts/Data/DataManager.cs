using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header("Save Game Settings")]
    private string saveFilePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveFilePath = Application.persistentDataPath + "/savegame.json";
    }

    public void SaveGame(int currentLevel, Vector2 checkpointPosition)
    {
        try
        {
            int currentHp = PlayerStats.Instance != null ? PlayerStats.Instance.CurrentHealth : 100;
            int currentMp = PlayerStats.Instance != null ? PlayerStats.Instance.CurrentMana : 50;

            PlayerData dataToSave = new PlayerData(currentLevel, checkpointPosition, currentHp, currentMp);

            string jsonText = JsonUtility.ToJson(dataToSave, true);
            File.WriteAllText(saveFilePath, jsonText);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi: Không thể lưu trò chơi! Chi tiết: " + e.Message + "");
        }
    }

    public PlayerData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonText = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<PlayerData>(jsonText);
        }
        return null;
    }

    public void SaveLevel(LevelData levelData, string fileName)
    {
        string folderPath = Application.dataPath + "/Resources/Levels";
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        if (!fileName.ToLower().EndsWith(".json")) fileName += ".json";

        string fullPath = Path.Combine(folderPath, fileName);
        string json = JsonUtility.ToJson(levelData, true);

        File.WriteAllText(fullPath, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public LevelData LoadLevel(string fileName)
    {
        string cleanName = fileName.Replace(".json", "");
        string resourcePath = "Levels/" + cleanName;
        TextAsset resourceFile = Resources.Load<TextAsset>(resourcePath);

        if (resourceFile != null) return JsonUtility.FromJson<LevelData>(resourceFile.text);
        else
        {
            Debug.LogError("Hệ thống: Không tìm thấy file map tại: Resources/" + resourcePath);
            return null;
        }
    }

    public string GetNextLevelName()
    {
        int maxLevel = 0;
        TextAsset[] files = Resources.LoadAll<TextAsset>("Levels");
        foreach (TextAsset file in files)
        {
            string[] parts = file.name.Split('_');
            if (parts.Length > 1 && int.TryParse(parts[1], out int num))
            {
                if (num > maxLevel) maxLevel = num;
            }
        }
        return "Level_" + (maxLevel + 1);
    }
    public LevelData CurrentEditingData { get; private set; }
    public int CurrentEditingLevelID { get; private set; } = 0;

    public void CreateBlankMapData(int width, int height)
    {
        CurrentEditingLevelID = 0;
        CurrentEditingData = new LevelData();
        for (int y = 0; y < height; y++)
        {
            MapRow row = new MapRow();
            for (int x = 0; x < width; x++) row.columns.Add(new TileData(99)); 
            CurrentEditingData.grid.Add(row);
        }
    }

    public bool UpdateTileData(int x, int y, int newTileID)
    {
        if (CurrentEditingData == null) return false;

        if (y >= 0 && y < CurrentEditingData.grid.Count && x >= 0 && x < CurrentEditingData.grid[y].columns.Count)
        {
            if (CurrentEditingData.grid[y].columns[x].id != newTileID)
            {
                CurrentEditingData.grid[y].columns[x].id = newTileID;
                return true; 
            }
        }
        return false;
    }

    public void SaveCurrentEditedMap()
    {
        if (CurrentEditingData == null) return;

        string fileName = (CurrentEditingLevelID == 0) ? GetNextLevelName() : "Level_" + CurrentEditingLevelID;
        if (CurrentEditingLevelID == 0) CurrentEditingLevelID = int.Parse(fileName.Split('_')[1]);

        SaveLevel(CurrentEditingData, fileName);
        Debug.Log("Hệ thống: ĐÃ LƯU MAP THÀNH CÔNG: " + fileName);
    }

    public LevelData LoadMapForEditing(string levelName)
    {
        LevelData data = LoadLevel(levelName);
        if (data != null)
        {
            CurrentEditingData = data;

 
            string[] parts = levelName.Split('_');
            if (parts.Length > 1 && int.TryParse(parts[1], out int parsedID))
            {
                CurrentEditingLevelID = parsedID;
            }
        }
        return data;
    }
}