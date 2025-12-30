using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public float gameTime = 0f;
    public float initialWallSpeed = 5f;
    public float maxWallSpeed = 15f;
    public float speedIncreaseRate = 0.1f;
    public float initialSpawnInterval = 3f;
    public float minSpawnInterval = 1f;
    public float intervalDecreaseRate = 0.02f;

    [HideInInspector] public float currentWallSpeed;
    [HideInInspector] public float currentSpawnInterval;

    void Start()
    {
        currentWallSpeed = initialWallSpeed;
        currentSpawnInterval = initialSpawnInterval;
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        currentWallSpeed = Mathf.Min(initialWallSpeed + (gameTime * speedIncreaseRate), maxWallSpeed);
        currentSpawnInterval = Mathf.Max(initialSpawnInterval - (gameTime * intervalDecreaseRate), minSpawnInterval);
    }

    public int GetAvailableColorCount()
    {
        if (gameTime < 15f) return 2;
        else if (gameTime < 30f) return 3;
        else if (gameTime < 45f) return 4;
        else return 5;
    }
}