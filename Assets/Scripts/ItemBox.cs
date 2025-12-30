using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 3f;
    private Vector2 direction;

    // 화면 경계 (게임 화면 크기에 맞춰 조절 가능)
    private float minX = -8f, maxX = 8f;
    private float minY = -4.5f, maxY = 4.5f;

    void Start()
    {
        // 시작 시 랜덤한 360도 방향 설정
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

        // 너무 오래 안 먹으면 사라지게 설정 (선택 사항)
        Destroy(gameObject, 15f);
    }

    void Update()
    {
        // 1. 독립적인 방향과 속도로 이동
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // 2. 화면 벽에 부딪히면 반사(튕기기)
        Vector3 pos = transform.position;

        if (pos.x < minX || pos.x > maxX)
        {
            direction.x *= -1; // X축 방향 반전
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
        }

        if (pos.y < minY || pos.y > maxY)
        {
            direction.y *= -1; // Y축 방향 반전
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }

        transform.position = pos;

        // 3. 둥실거리는 시각적 효과
        transform.localScale = Vector3.one * (1f + Mathf.Sin(Time.time * 3f) * 0.1f);
    }
}