using UnityEngine;
using System.Collections.Generic;

public class WallSpawner : MonoBehaviour
{
    public GameObject[] wallPrefabs;
    public GameObject itemBoxPrefab;

    [Header("설정")]
    public float spawnX = 12f;
    [Range(0f, 1f)]
    public float itemSpawnChance = 0.1f; // 10% 확률

    private float timer;

    void Update()
    {
        // 보스전일 때는 일반 소행성 스폰 중단
        if (BossManager.Instance != null && BossManager.Instance.IsBossActive()) 
        {
            timer = 0;
            return;
        }

        DifficultyManager dm = Object.FindFirstObjectByType<DifficultyManager>();
        float interval = (dm != null) ? dm.currentSpawnInterval : 3f;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0;
            SpawnFullLine();
            TrySpawnItem(); // 아이템 스폰 시도 추가
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

    void TrySpawnItem() //
    {
        if (Random.value < itemSpawnChance)
        {
            Vector3 itemPos = new Vector3(Random.Range(-5f, 5f), Random.Range(-3f, 3f), 0);
            Instantiate(itemBoxPrefab, itemPos, Quaternion.identity);
        }
    }
}