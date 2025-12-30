using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float stepSize = 1.5f;
    private int currentLane = 0;
    public int maxLane = 2;

    [Header("색상 데이터")]
    public string currentTag = "Red";
    public string[] colors = { "Red", "Yellow", "Green", "Blue", "Purple" };

    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        SetColor(0); // 시작 시 빨간색(0번) 설정
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && currentLane < maxLane) { currentLane++; UpdatePosition(); }
        if (Input.GetKeyDown(KeyCode.DownArrow) && currentLane > -maxLane) { currentLane--; UpdatePosition(); }
    }

    void UpdatePosition()
    {
        transform.position = new Vector3(transform.position.x, currentLane * stepSize, 0);
    }

    // Collision에서 던져주는 숫자를 받아서 처리
    public void SetColor(int index)
    {
        currentTag = colors[index];
        if (anim != null) anim.SetInteger("ColorIndex", index);
    }
}