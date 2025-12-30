using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI; // TextMeshPro 사용 시

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    [Header("UI & Effect")]
    public GameObject warningUI;      // "WARNING" 텍스트 오브젝트
    public Image flashPanel;
    public GameObject energyBlastPrefab;
    
    [Header("Boss Settings")]
    public GameObject bossObject;     // 거대 행성 프리팹
    public Transform bossSpawnPos;    // 거대 행성 등장 위치
    public GameObject[] wallPrefabs; // 기존 소행성 프리팹들 연결
    public float throwInterval = 0.1f; // 소행성 던지는 간격
    
    private bool isBossPhase = false;
    private int hitCount = 0;
    private PlayerCollision player;

    void Awake() => Instance = this;

    void Start() {
        player = Object.FindFirstObjectByType<PlayerCollision>();
        if(warningUI != null) warningUI.SetActive(false);
        if(bossObject != null) bossObject.SetActive(false);
        
        // 플래시 효과 투명하게 시작
        if(flashPanel != null) SetFlashAlpha(0f);
    }

    public void StartBossRaid() {
        if (isBossPhase) return;
        StartCoroutine(BossSequence());
    }

    IEnumerator BossSequence() {
        isBossPhase = true;

        // [추가] 화면에 있는 모든 소행성 제거
        ClearAllWalls();
        
        // 1. 경고 연출 (반짝임)
        if(warningUI != null) warningUI.SetActive(true);

        for(int i = 0; i < 3; i++) {
            SetFlashAlpha(0.4f);
            yield return new WaitForSecondsRealtime(0.3f);
            SetFlashAlpha(0f);
            yield return new WaitForSecondsRealtime(0.3f);
        }
        if(warningUI != null) warningUI.SetActive(false);

        // 2. 보스 등장
        if(bossObject != null) bossObject.SetActive(true);

        // 2. 보스 등장 및 소행성 던지기 시작
        bossObject.SetActive(true);
        StartCoroutine(ThrowPattern()); // 던지기 패턴 시작

        // 3. 연타 페이즈 시작 (스페이스바 입력 대기)
        hitCount = 0;
        while (player.gauge > 0) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                hitCount++;
                // 연타 시 보스가 살짝 흔들리는 효과 등을 여기에 추가하면 좋습니다.
                // 1. 에너지파 생성 (모션)
                SpawnEnergyBlast();
                
                // 2. 보스 흔들림 (타격감)
                StartCoroutine(ShakeBoss(0.1f, 0.1f));

                Debug.Log("Hit! 연타수: " + hitCount);
            }
            
            // 게이지 소모 (초당 20씩 감소 -> 약 5초 지속)
            player.gauge -= Time.unscaledDeltaTime * 20f; 
            yield return null;
        }

        EndBossRaid();
    }

    // 에너지파 생성 함수
    void SpawnEnergyBlast() {
        if (energyBlastPrefab != null && player != null) {
            // 플레이어 위치에서 에너지파 생성
            Instantiate(energyBlastPrefab, player.transform.position, Quaternion.identity);
        }
    }

    // 보스 흔들림 코루틴 (타격감 핵심)
    IEnumerator ShakeBoss(float duration, float magnitude) {
        if (bossObject == null) yield break;
        
        Vector3 originalPos = bossObject.transform.position;
        float elapsed = 0.0f;
        
        while (elapsed < duration) {
            float y = Random.Range(-1f, 1f) * magnitude;
            float x = Random.Range(-1f, 1f) * magnitude;
            
            bossObject.transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        bossObject.transform.position = originalPos;
    }

    // 소행성 일괄 제거 함수
    void ClearAllWalls() {
        // WallObject 스크립트가 붙은 모든 오브젝트를 찾아서 삭제
        WallObject[] walls = Object.FindObjectsByType<WallObject>(FindObjectsSortMode.None);
        foreach (WallObject wall in walls) {
            // Instantiate(explosionPrefab, wall.transform.position, Quaternion.identity);
            Destroy(wall.gameObject);
        }
        Debug.Log("모든 소행성 제거 완료");
    }

    IEnumerator ThrowPattern() {
        while (isBossPhase) {
            yield return new WaitForSecondsRealtime(throwInterval);
            if (!isBossPhase || wallPrefabs.Length == 0) yield break;

            // 무작위 색상 소행성 선택
            int randomIdx = Random.Range(0, wallPrefabs.Length);
            
            // [수정] 플레이어의 PlayerMovement 컴포넌트에서 현재 좌표를 정확히 가져옵니다.
            PlayerMovement pm = player.GetComponent<PlayerMovement>();
            float targetY = 0f;
            
            if (pm != null) {
                // 플레이어의 실제 현재 Y 위치를 타겟팅 (레인 시스템 좌표 반영)
                targetY = pm.transform.position.y;
            }
            
            // 보스 위치(오른쪽)에서 플레이어의 현재 Y축 라인으로 발사
            Vector3 spawnPos = new Vector3(10f, targetY, 0); 
            
            Instantiate(wallPrefabs[randomIdx], spawnPos, Quaternion.identity);
        }
    }

    void EndBossRaid() {
        isBossPhase = false;
        StopAllCoroutines();
        if(bossObject != null) bossObject.SetActive(false);
        
        // 점수 정산 (연타 1회당 50점)
        int bonus = hitCount * 50;
        player.score += bonus;
        player.gauge = 0;
        
        Debug.Log($"레이드 종료! 보너스 점수: {bonus}");
    }
// [변경] UI Image 전용 알파값 조절 함수
    void SetFlashAlpha(float alpha) {
        if(flashPanel == null) return;
        Color c = flashPanel.color;
        c.a = alpha;
        flashPanel.color = c;
    }

    public bool IsBossActive() => isBossPhase;
}