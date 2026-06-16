using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int currentLevel;
    public float positionX;
    public float positionY;
    public int currentHealth;

    public PlayerData(int level, Vector2 pos, int health)
    {
        currentLevel = level;
        positionX = pos.x;
        positionY = pos.y;
        currentHealth = health;
    }
}