using UnityEngine;
using System.Collections.Generic;

public class WallSpawner : MonoBehaviour
{
    public GameObject[] wallPrefabs;
    public float spawnX = 12f;
    private float timer;

    void Update()
    {
        DifficultyManager dm = Object.FindFirstObjectByType<DifficultyManager>();
        float interval = (dm != null) ? dm.currentSpawnInterval : 3f;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0;
            SpawnFullLine();
        }
    }

    void SpawnFullLine()
    {
        List<int> laneY = new List<int> { -2, -1, 0, 1, 2 };
        for (int i = 0; i < 5; i++)
        {
            int randomLaneIdx = Random.Range(0, laneY.Count);
            int yPos = laneY[randomLaneIdx];
            laneY.RemoveAt(randomLaneIdx);
            Vector3 spawnPos = new Vector3(spawnX, yPos * 1.5f, 0);
            Instantiate(wallPrefabs[i], spawnPos, Quaternion.identity);
        }
    }
}