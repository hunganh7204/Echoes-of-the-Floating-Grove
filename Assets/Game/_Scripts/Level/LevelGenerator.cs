using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    DeadZone = 0,
    Start = -1,
    Finish = -2,
    Ground = 1,
    Checkpoint = 2,
    Enemy_Slime = 3,
    TreasureChest = 4,
    NPC = 5,
    FlyingEnemy = 6,
    Empty = 99
}

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance { get; private set; }
    public int CurrentLevel { get; private set; }

    [Header("Player Reference")]
    [SerializeField] private GameObject player;

    [Header("Prefabs (2D)")]
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject checkpointPrefab;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private GameObject finishPrefab;
    [SerializeField] private GameObject deadZonePrefab;
    [SerializeField] private GameObject flyingEnemyPrefab;

    [Header("Settings")]
    public float tileSize = 1f;
    public Transform levelParent;
    public LevelData currentLevelData;

    private Dictionary<TileType, Queue<GameObject>> poolDictionary = new Dictionary<TileType, Queue<GameObject>>();
    private Dictionary<GameObject, TileType> activeTiles = new Dictionary<GameObject, TileType>();

    private Vector3 startPosition;
    private bool hasStartPos = false;

    public Vector3 GetStartPosition() => startPosition;
    public bool HasStartPosition() => hasStartPos;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void BuildLevelFromData(LevelData data)
    {
        currentLevelData = data;
        if (currentLevelData == null || currentLevelData.grid == null) return;

        ClearMapToPool();
        hasStartPos = false;

        int lengthY = currentLevelData.grid.Count;

        for (int y = 0; y < lengthY; y++)
        {
            MapRow row = currentLevelData.grid[y];
            for (int x = 0; x < row.columns.Count; x++)
            {
                TileData tileNode = row.columns[x];
                TileType currentType = (TileType)tileNode.id;

                if (currentType == TileType.Empty) continue;

                Vector3 spawnPos = new Vector3(x * tileSize, y * tileSize, 0f);
                GameObject prefabToUse = null;

                if (currentType == TileType.Start)
                {
                    startPosition = spawnPos;
                    hasStartPos = true;
                }

                switch (currentType)
                {
                    case TileType.Start: prefabToUse = startPrefab; break;
                    case TileType.Ground: prefabToUse = groundPrefab; break;
                    case TileType.Checkpoint: prefabToUse = checkpointPrefab; break;
                    case TileType.Enemy_Slime: prefabToUse = slimePrefab; break;
                    case TileType.TreasureChest: prefabToUse = chestPrefab; break;
                    case TileType.NPC: prefabToUse = npcPrefab; break;
                    case TileType.Finish: prefabToUse = finishPrefab; break;
                    case TileType.DeadZone: prefabToUse = deadZonePrefab; break;
                    case TileType.FlyingEnemy: prefabToUse = flyingEnemyPrefab; break;
                }

                if (prefabToUse != null)
                {
                    GetFromPool(currentType, prefabToUse, spawnPos);
                }
            }
        }
    }

    private GameObject GetFromPool(TileType tileType, GameObject prefab, Vector3 position)
    {
        if (!poolDictionary.ContainsKey(tileType))
        {
            poolDictionary[tileType] = new Queue<GameObject>();
        }

        GameObject tile;

        if (poolDictionary[tileType].Count > 0)
        {
            tile = poolDictionary[tileType].Dequeue();
            tile.transform.position = position;
            tile.SetActive(true);
        }
        else
        {
            tile = Instantiate(prefab, position, Quaternion.identity);
            if (levelParent != null) tile.transform.SetParent(levelParent);
        }

        activeTiles.Add(tile, tileType);
        return tile;
    }

    public void ClearMapToPool()
    {
        foreach (var kvp in activeTiles)
        {
            GameObject obj = kvp.Key;
            TileType type = kvp.Value;

            if (obj != null)
            {
                obj.SetActive(false);
                if (!poolDictionary.ContainsKey(type))
                {
                    poolDictionary[type] = new Queue<GameObject>();
                }
                poolDictionary[type].Enqueue(obj);
            }
        }
        activeTiles.Clear();
    }

    public void LoadLevelAndSpawnPlayer(int levelID, Vector2? customSpawnPos = null)
    {
        CurrentLevel = levelID;
        LevelData data = DataManager.Instance.LoadLevel("Level_" + levelID);

        if (data != null)
        {
            BuildLevelFromData(data);

            Vector3 finalSpawnPos;
            if (customSpawnPos.HasValue)
            {
                finalSpawnPos = new Vector3(customSpawnPos.Value.x, customSpawnPos.Value.y, 0f);
            }
            else
            {
                finalSpawnPos = GetStartPosition();
            }

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            player.SetActive(true);
            if (rb != null) rb.linearVelocity = Vector2.zero;

            player.transform.position = finalSpawnPos;
            

            Debug.Log($"Hệ thống: Đã load xong Level {levelID} và thả nhân vật tại {finalSpawnPos}");
        }
        else
        {
            Debug.LogError("Hệ thống: Không tìm thấy file JSON của Level_" + levelID);
        }
    }
}
