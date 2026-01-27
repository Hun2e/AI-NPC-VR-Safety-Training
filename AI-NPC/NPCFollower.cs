using UnityEngine;

public class NPCFollower : MonoBehaviour
{
    public Transform player; // 플레이어 Transform (카메라 또는 XR Rig)
    public Vector3 offset = new Vector3(1.5f, 0.0f, 3.0f); // NPC 위치 오프셋
    public float fixedHeight = 2.0f; // NPC의 고정된 높이
    public float followSpeed = 5.0f; // NPC가 따라가는 속도
    public float rotationSpeed = 5.0f; // NPC 회전 속도

    private Transform playerCamera; // 플레이어의 카메라 Transform

    void Start()
    {
        // 플레이어의 카메라 Transform 검색
        if (player != null)
        {
            // 카메라를 자식으로 검색 (Main Camera 또는 XR Rig의 카메라)
            playerCamera = player.GetComponentInChildren<Camera>().transform;

            if (playerCamera == null)
            {
                Debug.LogError("플레이어에 연결된 카메라를 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.LogError("Player Transform이 연결되지 않았습니다!");
        }
    }

    void Update()
    {
        if (player != null && playerCamera != null)
        {
            // 플레이어의 카메라 방향을 기준으로 NPC 위치 설정
            Vector3 cameraForward = playerCamera.forward; 
            cameraForward.y = 0; // 수평 방향만 사용 (Y축 영향 제거)
            cameraForward.Normalize();

            // 오프셋을 카메라 방향에 따라 조정
            Vector3 dynamicOffset = cameraForward * offset.z + playerCamera.right * offset.x;
            
            // NPC가 플레이어와 같은 높이에서 위치하도록 수정
            Vector3 targetPosition = player.position + dynamicOffset;
            targetPosition.y = player.position.y + offset.y; // 플레이어의 높이 + 오프셋

            // NPC 위치를 부드럽게 이동
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

            // NPC가 플레이어를 바라보게 설정
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // NPC가 고정된 높이에서만 회전하도록 Y축 방향 제거
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

}
