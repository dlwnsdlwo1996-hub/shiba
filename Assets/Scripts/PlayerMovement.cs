using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float stepSize = 1.5f;
    private int currentLane = 0;
    public int maxLane = 2;

    [Header("잔상 설정")]
    public float ghostDelay = 0.05f;
    public float ghostDestroyTime = 0.2f;

    [Header("색상 데이터")]
    public string currentTag = "Red";
    public string[] colors = { "Red", "Yellow", "Green", "Blue", "Purple" };

    private Animator anim;
    private SpriteRenderer sr;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        gameObject.tag = "Red"; // 시작 태그 초기화
    }

    void Start()
    {
        SetColor(0);
    }

    void Update()
    {
        // 이동 시 잔상 루틴 추가
        if (Input.GetKeyDown(KeyCode.UpArrow) && currentLane < maxLane) 
        { 
            currentLane++; 
            UpdatePosition(); 
            StartCoroutine(CreateTrailRoutine());
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && currentLane > -maxLane) 
        { 
            currentLane--; 
            UpdatePosition(); 
            StartCoroutine(CreateTrailRoutine());
        }
    }

    void UpdatePosition()
    {
        transform.position = new Vector3(transform.position.x, currentLane * stepSize, 0);
    }

    // 팀원의 잔상 생성 로직 통합
    IEnumerator CreateTrailRoutine()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject ghost = new GameObject("GhostTrail");
            ghost.transform.position = transform.position;
            ghost.transform.rotation = transform.rotation;
            ghost.transform.localScale = transform.localScale;

            SpriteRenderer ghostSR = ghost.AddComponent<SpriteRenderer>();
            ghostSR.sprite = sr.sprite;
            ghostSR.color = new Color(1, 1, 1, 0.4f);
            ghostSR.sortingOrder = sr.sortingOrder - 1;

            Destroy(ghost, ghostDestroyTime);
            yield return new WaitForSeconds(ghostDelay);
        }
    }

    public void SetColor(int index)
    {
        if (index >= 0 && index < colors.Length)
        {
            currentTag = colors[index];
            gameObject.tag = currentTag; // 오브젝트 태그 실제 변경
            if (anim != null) anim.SetInteger("ColorIndex", index);
        }
    }
}