using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    // public을 붙여야 PlayerCollision에서 접근이 가능합니다.
    public float scrollSpeed = 0.1f;
    private MeshRenderer mr;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        // 배경이 흐르게 만드는 로직
        float offset = Time.time * scrollSpeed;
        mr.material.mainTextureOffset = new Vector2(offset, 0);
    }
}