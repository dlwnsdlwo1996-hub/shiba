using UnityEngine;

public class EnergyBlast : MonoBehaviour
{
    public float speed = 20f; // 에너지파 속도

    void Update()
    {
        // 오른쪽으로 이동 (unscaledDeltaTime을 사용하여 정지 상태 대응)
        transform.Translate(Vector3.right * speed * Time.unscaledDeltaTime);

        // 화면 밖으로 나가면 삭제
        if (transform.position.x > 15f)
        {
            Destroy(gameObject);
        }
    }
}