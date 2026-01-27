using UnityEngine;

public class VRMovement : MonoBehaviour
{
    public float moveSpeed = 2.0f;    // 이동 속도
    public float rotateSpeed = 45.0f; // 회전 속도

    void Update()
    {
        // 왼쪽 스틱 입력: 이동
        Vector2 moveInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 오른쪽 스틱 입력: 시점 회전
        Vector2 rotateInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        transform.Rotate(0, rotateInput.x * rotateSpeed * Time.deltaTime, 0);
    }
}
