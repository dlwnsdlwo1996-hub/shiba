using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public GameObject explosionPrefab;

    [Header("생존 시스템")]
    public int currentHp = 3;
    public int maxHp = 3;

    [Header("점수 및 게이지")]
    public int score = 0;
    public float gauge = 0f;
    public float gaugePerWall = 10f;

    private ScrollingBackground bg;

    void Start()
    {
        bg = Object.FindFirstObjectByType<ScrollingBackground>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm == null) return;

        // 무적 체크 제거: 오직 태그가 일치할 때만 성공 처리
        if (other.CompareTag(pm.currentTag))
        {
            ProcessSuccess(other, pm);
        }
        else if (!other.CompareTag("Untagged"))
        {
            ProcessFailure(other);
        }
    }

    void ProcessSuccess(Collider2D other, PlayerMovement pm)
    {
        score += 100;

        // 게이지 상승 로직만 유지 (추후 필살기 등으로 활용 가능)
        gauge = Mathf.Min(100f, gauge + gaugePerWall);

        if (explosionPrefab != null)
        {
            GameObject effect = Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);
            var main = effect.GetComponent<ParticleSystem>().main;
            main.startColor = other.GetComponent<SpriteRenderer>().color;
            Destroy(effect, 1f);
        }
        Destroy(other.gameObject);

        // 다음 색상 설정
        pm.SetColor(Random.Range(0, 5));
    }

    void ProcessFailure(Collider2D other)
    {
        currentHp--;
        Destroy(other.gameObject);

        if (currentHp <= 0)
        {
            Time.timeScale = 0f;
            Debug.Log("게임 오버");
        }
        else
        {
            Debug.Log("남은 체력: " + currentHp);
        }
    }
}