using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public int id = 99;

    public TileData(int id)
    {
        this.id = id;
    }
}

[System.Serializable]
public class MapRow
{
    public List<TileData> columns = new List<TileData>();
}

[System.Serializable]
public class LevelData
{
    public int levelID;
    public List<MapRow> grid = new List<MapRow>();
}
