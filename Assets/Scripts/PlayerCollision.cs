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

    [Header("무적 설정")]
    public bool isInvincible = false;
    public float invincibleDuration = 5.0f;
    private float invincibleTimer = 0f;

    private ScrollingBackground bg;
    private float originalScrollSpeed;

    void Start()
    {
        // 씬에서 스크롤 배경 스크립트를 찾아옵니다.
        bg = Object.FindFirstObjectByType<ScrollingBackground>();
        if (bg != null) originalScrollSpeed = bg.scrollSpeed;
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.unscaledDeltaTime;
            GetComponent<SpriteRenderer>().color = Color.HSVToRGB(Mathf.PingPong(Time.time * 5, 1), 1, 1);
            if (invincibleTimer <= 0) StopInvincibleMode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm == null) return;

        if (other.CompareTag(pm.currentTag) || isInvincible)
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
        if (!isInvincible)
        {
            gauge += gaugePerWall;
            if (gauge >= 100f) StartInvincibleMode();
        }

        if (explosionPrefab != null)
        {
            GameObject effect = Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);
            var main = effect.GetComponent<ParticleSystem>().main;
            main.startColor = other.GetComponent<SpriteRenderer>().color;
            Destroy(effect, 1f);
        }
        Destroy(other.gameObject);
        if (!isInvincible) pm.SetColor(Random.Range(0, 5));
    }

    void ProcessFailure(Collider2D other)
    {
        currentHp--; // 하트 감소
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

    void StartInvincibleMode()
    {
        isInvincible = true;
        invincibleTimer = invincibleDuration;
        Time.timeScale = 2.0f;
        if (bg != null) bg.scrollSpeed = originalScrollSpeed * 2.0f;
    }

    void StopInvincibleMode()
    {
        isInvincible = false;
        Time.timeScale = 1.0f;
        GetComponent<SpriteRenderer>().color = Color.white;
        if (bg != null) bg.scrollSpeed = originalScrollSpeed;
    }
}