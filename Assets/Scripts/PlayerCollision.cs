using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    public GameObject explosionPrefab;

    [Header("생존 및 게이지")]
    public int currentHp = 3;
    public float gauge = 0f;
    public float gaugePerWall = 10f;
    public Slider feverSlider;
    public int score = 0;

    [Header("아이템 및 무적")]
    public bool hasItem = false;
    public bool isInvincible = false;
    public float invincibleDuration = 3.0f;
    private float invincibleTimer = 0f;
    private bool isSlowingDown = false;

    private SpriteRenderer spriteRenderer;

    void Start() => spriteRenderer = GetComponent<SpriteRenderer>();

    void Update()
    {
        if (feverSlider != null) feverSlider.value = gauge;
        if (Input.GetKeyDown(KeyCode.LeftControl) && hasItem && !isInvincible) UseItem();
        if (isInvincible) HandleInvincible();
    }

    void HandleInvincible()
    {
        invincibleTimer -= Time.unscaledDeltaTime;
        float hue = Mathf.PingPong(Time.time * 3f, 1f);
        spriteRenderer.color = Color.HSVToRGB(hue, 0.9f, 1f);

        if (invincibleTimer <= 0) StartCoroutine(SmoothStopInvincible());
    }

    void UseItem()
    {
        hasItem = false;
        isInvincible = true;
        invincibleTimer = invincibleDuration;
        Time.timeScale = 1.8f;
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
        if (other.CompareTag("ItemBox"))
        {
            if (!hasItem) hasItem = true;
            Destroy(other.gameObject);
            return;
        }

        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm == null) return;

        // 성공 판정: 색상 일치 OR 무적 상태
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
        if (BossManager.Instance != null && BossManager.Instance.IsBossActive())
        {
            gauge = Mathf.Min(100f, gauge + 10f); // 보스전 게이지 회복
            score += 200;
        }
        else
        {
            score += 100;
            gauge = Mathf.Min(100f, gauge + gaugePerWall);
            if (gauge >= 100f) BossManager.Instance.StartBossRaid();
        }

        SpawnExplosion(other);
        Destroy(other.gameObject);
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