using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float stepSize = 1.5f;
    private int currentLane = 0;
    public int maxLane = 2;

    [Header("잔상 설정")]
    public float ghostDelay = 0.05f;      // 잔상 생성 간격 (더 촘촘하게 수정)
    public float ghostDestroyTime = 0.5f; // 잔상이 사라지는 시간
    public Color ghostColor = new Color(1, 1, 1, 0.5f); // 잔상 색상/투명도

    [Header("색상 데이터")]
    public string currentTag = "Red";
    public string[] tagNames = { "Red", "Yellow", "Green", "Blue", "Purple" };

    private Animator anim;
    private SpriteRenderer sr;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        gameObject.tag = "Red"; 
    }

    void Start() => SetColor(0);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && currentLane < maxLane)
        {
            currentLane++;
            UpdatePosition();
            StartCoroutine(CreateTrailRoutine()); // 잔상 루틴 실행
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && currentLane > -maxLane)
        {
            currentLane--;
            UpdatePosition();
            StartCoroutine(CreateTrailRoutine());
        }
    }

    void UpdatePosition() => transform.position = new Vector3(transform.position.x, currentLane * stepSize, 0);

    // [보완된 잔상 로직]
    IEnumerator CreateTrailRoutine()
    {
        // 이동 직후 0.1~0.2초 동안 짧게 여러 개의 잔상을 남김
        float elapsed = 0f;
        while (elapsed < 0.15f) 
        {
            GameObject ghost = new GameObject("GhostTrail");
            
            // 1. 위치 및 회전값 복사
            ghost.transform.position = transform.position;
            ghost.transform.rotation = transform.rotation;
            ghost.transform.localScale = transform.localScale;

            // 2. SpriteRenderer 설정
            SpriteRenderer ghostSR = ghost.AddComponent<SpriteRenderer>();
            
            // 현재 플레이어가 애니메이션 중이더라도 '현재 프레임'의 이미지를 그대로 복사
            ghostSR.sprite = sr.sprite; 
            ghostSR.color = ghostColor;
            
            // 플레이어보다 살짝 뒤에 보이도록 설정
            ghostSR.sortingLayerName = sr.sortingLayerName;
            ghostSR.sortingOrder = sr.sortingOrder - 1;

            // 3. 서서히 사라지는 효과 (선택 사항)
            StartCoroutine(FadeOutGhost(ghostSR));

            Destroy(ghost, ghostDestroyTime);
            
            yield return new WaitForSeconds(ghostDelay);
            elapsed += ghostDelay;
        }
    }

    // 잔상이 자연스럽게 사라지게 하는 추가 루틴
    IEnumerator FadeOutGhost(SpriteRenderer ghostSR)
    {
        float fadeSpeed = 1f / ghostDestroyTime;
        Color c = ghostSR.color;
        while (c.a > 0)
        {
            c.a -= Time.deltaTime * fadeSpeed;
            if(ghostSR == null) break;
            ghostSR.color = c;
            yield return null;
        }
    }

    public void SetColor(int index)
    {
        if (index >= 0 && index < tagNames.Length)
        {
            currentTag = tagNames[index];
            gameObject.tag = currentTag;
            if (anim != null) anim.SetInteger("ColorIndex", index);
        }
    }
}