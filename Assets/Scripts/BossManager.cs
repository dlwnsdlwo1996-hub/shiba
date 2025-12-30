using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    [Header("UI & Effect")]
    public GameObject warningUI;
    public Image flashPanel;
    public GameObject energyBlastPrefab; // 추가: 에너지파 프리팹

    [Header("Boss Settings")]
    public GameObject bossObject;
    public GameObject[] wallPrefabs;
    public float throwInterval = 1.5f;

    private bool isBossPhase = false;
    private int hitCount = 0;
    private PlayerCollision player;

    void Awake() => Instance = this;

    void Start()
    {
        player = Object.FindFirstObjectByType<PlayerCollision>();
    }

    public void StartBossRaid()
    {
        if (isBossPhase) return;
        StartCoroutine(BossSequence());
    }

    IEnumerator BossSequence()
    {
        isBossPhase = true;
        ClearAllWalls();

        // 1. 경고 및 보스 등장
        if(warningUI != null) warningUI.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        if(warningUI != null) warningUI.SetActive(false);
        bossObject.SetActive(true);

        StartCoroutine(ThrowPattern()); // 소행성 패턴 시작

        // 2. 연타 페이즈
        hitCount = 0;
        while (player.gauge > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                hitCount++;
                SpawnEnergyBlast(); // 에너지파 발사
                StartCoroutine(ShakeBoss(0.1f, 0.1f)); // 타격감
            }
            player.gauge -= Time.unscaledDeltaTime * 20f;
            yield return null;
        }
        EndBossRaid();
    }

    IEnumerator ThrowPattern() // 플레이어 위치로 소행성 던지기
    {
        while (isBossPhase)
        {
            yield return new WaitForSecondsRealtime(throwInterval);
            int randomIdx = Random.Range(0, wallPrefabs.Length);
            Vector3 spawnPos = new Vector3(10f, player.transform.position.y, 0);
            Instantiate(wallPrefabs[randomIdx], spawnPos, Quaternion.identity);
        }
    }

    void SpawnEnergyBlast()
    {
        if (energyBlastPrefab != null)
            Instantiate(energyBlastPrefab, player.transform.position, Quaternion.identity);
    }

    IEnumerator ShakeBoss(float duration, float magnitude)
    {
        Vector3 originalPos = bossObject.transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            bossObject.transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        bossObject.transform.position = originalPos;
    }

    void EndBossRaid()
    {
        isBossPhase = false;
        bossObject.SetActive(false);
        player.score += hitCount * 50;
        player.gauge = 0;
    }

    void ClearAllWalls()
    {
        WallObject[] walls = Object.FindObjectsByType<WallObject>(FindObjectsSortMode.None);
        foreach (var wall in walls) Destroy(wall.gameObject);
    }

    public bool IsBossActive() => isBossPhase;
}