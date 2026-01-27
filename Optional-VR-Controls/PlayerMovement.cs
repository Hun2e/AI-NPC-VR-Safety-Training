using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f; // 이동 속도
    public float rotationSpeed = 700.0f; // 회전 속도

    void Update()
    {
        // 이동 (WASD 키)
        float moveHorizontal = Input.GetAxis("Horizontal"); // A, D 키
        float moveVertical = Input.GetAxis("Vertical"); // W, S 키

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
        transform.Translate(movement * speed * Time.deltaTime, Space.World);

        // 회전 (마우스)
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
