using UnityEngine;

public class WallObject : MonoBehaviour
{
    void Update()
    {
        DifficultyManager dm = Object.FindFirstObjectByType<DifficultyManager>();
        float speed = (dm != null) ? dm.currentWallSpeed : 5f;

        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}