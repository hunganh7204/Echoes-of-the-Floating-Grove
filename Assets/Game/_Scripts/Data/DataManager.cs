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

    #region PHẦN 1: QUẢN LÝ TIẾN TRÌNH CHƠI (SAVE/LOAD CHECKPOINT)
    // =========================================================================
    public void SaveGame(int currentLevel, Vector2 checkpointPosition)
    {
        try
        {
            // Truyền currentLevel vào PlayerData
            PlayerData dataToSave = new PlayerData(currentLevel, checkpointPosition, 100);

            string jsonText = JsonUtility.ToJson(dataToSave, true);
            File.WriteAllText(saveFilePath, jsonText);
            Debug.Log($"<color=cyan>Hệ thống: Đã lưu trò chơi thành công tại: {saveFilePath}</color>");
        }
        catch (System.Exception e)
        {
            Debug.LogError("<color=red>Hệ thống Lỗi: Không thể lưu trò chơi! Chi tiết: " + e.Message + "</color>");
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
    #endregion

    #region PHẦN 2: QUẢN LÝ MÀN CHƠI (ĐỌC / GHI FILE JSON)
    // =========================================================================
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
    #endregion

    #region PHẦN 3: XỬ LÝ DỮ LIỆU EDITOR (RUNTIME DATA MANIPULATION)
    // =========================================================================

    // Lưu trữ dữ liệu map đang được vẽ
    public LevelData CurrentEditingData { get; private set; }
    public int CurrentEditingLevelID { get; private set; } = 0;

    // Tạo một bản đồ trắng tinh
    public void CreateBlankMapData(int width, int height)
    {
        CurrentEditingLevelID = 0;
        CurrentEditingData = new LevelData();
        for (int y = 0; y < height; y++)
        {
            MapRow row = new MapRow();
            for (int x = 0; x < width; x++) row.columns.Add(new TileData(99)); // 99 = Empty
            CurrentEditingData.grid.Add(row);
        }
    }

    // Đổi ID của 1 ô gạch. Trả về TRUE nếu thực sự có sự thay đổi.
    public bool UpdateTileData(int x, int y, int newTileID)
    {
        if (CurrentEditingData == null) return false;

        // Kiểm tra xem x, y có nằm trong giới hạn mảng không
        if (y >= 0 && y < CurrentEditingData.grid.Count && x >= 0 && x < CurrentEditingData.grid[y].columns.Count)
        {
            if (CurrentEditingData.grid[y].columns[x].id != newTileID)
            {
                CurrentEditingData.grid[y].columns[x].id = newTileID;
                return true; // Báo hiệu cho Editor biết mảng đã thay đổi
            }
        }
        return false;
    }

    // Tự động xử lý logic lấy tên và lưu file
    public void SaveCurrentEditedMap()
    {
        if (CurrentEditingData == null) return;

        string fileName = (CurrentEditingLevelID == 0) ? GetNextLevelName() : "Level_" + CurrentEditingLevelID;
        if (CurrentEditingLevelID == 0) CurrentEditingLevelID = int.Parse(fileName.Split('_')[1]);

        SaveLevel(CurrentEditingData, fileName);
        Debug.Log("Hệ thống: ĐÃ LƯU MAP THÀNH CÔNG: " + fileName);
    }

    // Load một file vào bộ nhớ để chuẩn bị vẽ tiếp
    public LevelData LoadMapForEditing(string levelName)
    {
        LevelData data = LoadLevel(levelName);
        if (data != null)
        {
            CurrentEditingData = data;

            // Cập nhật lại ID đang sửa
            string[] parts = levelName.Split('_');
            if (parts.Length > 1 && int.TryParse(parts[1], out int parsedID))
            {
                CurrentEditingLevelID = parsedID;
            }
        }
        return data;
    }
    #endregion
}