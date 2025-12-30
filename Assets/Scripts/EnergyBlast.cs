using UnityEngine;

public class EnergyBlast : MonoBehaviour
{
    public float speed = 25f; // 날아가는 속도

        void Update() {
            // 오른쪽(보스 방향)으로 이동
            transform.Translate(Vector3.right * speed * Time.unscaledDeltaTime);

            // 화면 밖으로 나가면 자동 삭제
            if (transform.position.x > 15f) Destroy(gameObject);
        }
}
