using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    public Slider feverSlider;

    [Header("아이템 및 무적")]
    public bool hasItem = false;
    public bool isInvincible = false;
    public float invincibleDuration = 3.0f;
    private float invincibleTimer = 0f;
    private bool isSlowingDown = false;

    private SpriteRenderer spriteRenderer;
    private float originalFixedDeltaTime;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update()
    {
        // UI 업데이트
        if (feverSlider != null) feverSlider.value = gauge;

        // 아이템 사용 (Ctrl 키)
        if (Input.GetKeyDown(KeyCode.LeftControl) && hasItem && !isInvincible)
        {
            UseItem();
        }

        // 무적 상태 연출 및 타이머
        if (isInvincible)
        {
            HandleInvincible();
        }
    }

    void HandleInvincible()
    {
        invincibleTimer -= Time.unscaledDeltaTime;
        float hue = Mathf.PingPong(Time.time * 3f, 1f);
        spriteRenderer.color = Color.HSVToRGB(hue, 0.9f, 1f);

        if (invincibleTimer <= 1.0f) // 종료 직전 깜빡임
        {
            float blink = Mathf.PingPong(Time.time * 20f, 1f);
            Color c = spriteRenderer.color;
            c.a = (blink > 0.5f) ? 1f : 0.3f;
            spriteRenderer.color = c;
        }

        if (invincibleTimer <= 0) StartCoroutine(SmoothStopInvincible());
    }

    void UseItem()
    {
        hasItem = false;
        isInvincible = true;
        invincibleTimer = invincibleDuration;
        Time.timeScale = 1.8f; // 무적 시 속도 증가
    }

    IEnumerator SmoothStopInvincible()
    {
        isInvincible = false;
        isSlowingDown = true;
        spriteRenderer.color = Color.white;
        float elapsed = 0f;
        while (elapsed < 1.2f)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(0.4f, 1.0f, elapsed / 1.2f);
            yield return null;
        }
        Time.timeScale = 1.0f;
        isSlowingDown = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ItemBox")) // 아이템 획득
        {
            if (!hasItem) hasItem = true;
            Destroy(other.gameObject);
            return;
        }

        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm == null) return;

        // 성공 판정 조건 통합 (색상 일치 OR 무적 OR 슬로우 중)
        if (other.CompareTag(pm.currentTag) || isInvincible || isSlowingDown)
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
        // 보스전 게이지 로직 통합
        if (BossManager.Instance != null && BossManager.Instance.IsBossActive()) 
        {
            gauge = Mathf.Min(100f, gauge + 10f);
            score += 200;
        } 
        else 
        {
            score += 100;
            gauge = Mathf.Min(100f, gauge + gaugePerWall);
            if (gauge >= 100f && !BossManager.Instance.IsBossActive()) BossManager.Instance.StartBossRaid();
        }

        // 이펙트 및 파괴
        SpawnExplosion(other);
        Destroy(other.gameObject);

        // 무적/슬로우 아닐 때만 색상 변경
        if (!isInvincible && !isSlowingDown) pm.SetColor(Random.Range(0, 5));
    }

    void SpawnExplosion(Collider2D other)
    {
        if (explosionPrefab != null)
        {
            GameObject effect = Instantiate(explosionPrefab, other.transform.position, Quaternion.identity);
            var main = effect.GetComponent<ParticleSystem>().main;
            main.startColor = other.GetComponent<SpriteRenderer>().color;
            Destroy(effect, 1f);
        }
    }

    void ProcessFailure(Collider2D other)
    {
        currentHp--;
        Destroy(other.gameObject);
        if (currentHp <= 0) Time.timeScale = 0f;
    }
}