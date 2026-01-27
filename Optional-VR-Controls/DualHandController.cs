using UnityEngine;

public class DualHandController : MonoBehaviour
{
    public Transform rightHandAnchor; // 오른손 기준점
    public Transform leftHandAnchor;  // 왼손 기준점

    private GameObject rightGrabbedObject = null; // 오른손으로 잡은 오브젝트
    private GameObject leftGrabbedObject = null;  // 왼손으로 잡은 오브젝트

    private float grabRange = 2.0f; // 잡기 거리

    void Update()
    {
        // 오른손 트리거 버튼: 선택 기능
        HandleTriggerInput(OVRInput.Controller.RTouch, rightHandAnchor);

        // 왼손 트리거 버튼: 선택 기능
        HandleTriggerInput(OVRInput.Controller.LTouch, leftHandAnchor);

        // 오른손 그립 버튼: 잡기 기능
        HandleGrabInput(OVRInput.Controller.RTouch, rightHandAnchor, ref rightGrabbedObject);

        // 왼손 그립 버튼: 잡기 기능
        HandleGrabInput(OVRInput.Controller.LTouch, leftHandAnchor, ref leftGrabbedObject);
    }

    // 트리거 버튼 기능 (선택)
    void HandleTriggerInput(OVRInput.Controller controller, Transform handAnchor)
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            RaycastHit hit;
            if (Physics.Raycast(handAnchor.position, handAnchor.forward, out hit, grabRange))
            {
                Debug.Log($"선택된 오브젝트: {hit.collider.gameObject.name}");
                // 선택된 오브젝트에 대한 동작
                hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }

    // 그립 버튼 기능 (잡기 및 놓기)
    void HandleGrabInput(OVRInput.Controller controller, Transform handAnchor, ref GameObject grabbedObject)
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
        {
            if (grabbedObject == null) // 아직 잡은 오브젝트가 없으면
            {
                RaycastHit hit;
                if (Physics.Raycast(handAnchor.position, handAnchor.forward, out hit, grabRange))
                {
                    if (hit.collider.gameObject.CompareTag("Grabbable"))
                    {
                        grabbedObject = hit.collider.gameObject;
                        grabbedObject.transform.SetParent(handAnchor);
                        grabbedObject.GetComponent<Rigidbody>().isKinematic = true; // 물리 비활성화
                        Debug.Log($"잡기: {grabbedObject.name}");
                    }
                }
            }
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, controller))
        {
            if (grabbedObject != null)
            {
                grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                grabbedObject.transform.SetParent(null);
                grabbedObject = null;
                Debug.Log("놓기 완료");
            }
        }
    }
}
