using UnityEngine;

public class WallObject : MonoBehaviour
{
    public float speed = 5f;
    void Update()
    {
        if (BossManager.Instance != null && BossManager.Instance.IsBossActive())
        {
            speed = 12f;
        } else
        {
            DifficultyManager dm = Object.FindFirstObjectByType<DifficultyManager>();
            speed = (dm != null) ? dm.currentWallSpeed : 5f;
        }

        transform.Translate(Vector3.left * speed * Time.unscaledDeltaTime);
        // transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}