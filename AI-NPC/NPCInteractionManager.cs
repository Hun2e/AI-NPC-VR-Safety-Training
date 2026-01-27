using UnityEngine;

// AI NPC의 전체 상호작용 흐름을 관리하는 클래스
// 사용자 음성 입력 -> 응답 생성 -> 상태 분기 -> NPC 출력까지의 흐름 담당
public class NPCInteractionManager : MonoBehaviour
{
    public VoiceRecorder voiceRecorder; // VoiceRecorder를 직접 연결
    public Transform player; // 플레이어 Transform

    private bool isRecording = false; // 녹음 상태를 확인하기 위한 변수

    void Start()
    {
        if (voiceRecorder == null)
        {
            Debug.LogError("VoiceRecorder가 연결되지 않았습니다!");
        }

        // 연결된 컨트롤러 확인
        string[] controllers = Input.GetJoystickNames();
        foreach (string controller in controllers)
        {
            if (!string.IsNullOrEmpty(controller))
            {
                Debug.Log($"연결된 컨트롤러: {controller}");
            }
        }
    }

    void Update()
    {
        // X 버튼 (왼손 컨트롤러) 누름 시작 시 녹음 시작
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch) && !isRecording)
        {
            StartRecording();
        }

        // X 버튼 (왼손 컨트롤러)에서 손을 뗄 때 녹음 중지 및 처리 시작
        if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.LTouch) && isRecording)
        {
            StopRecordingAndProcess();
        }
    }

    private void StartRecording()
    {
        if (isRecording) return; // 이미 녹음 중이라면 중복 실행 방지

        if (voiceRecorder != null)
        {
            voiceRecorder.StartRecording();
            isRecording = true; // 녹음 상태 업데이트
            Debug.Log("녹음 시작!");
        }
        else
        {
            Debug.LogError("VoiceRecorder가 연결되지 않았습니다!");
        }
    }

    private void StopRecordingAndProcess()
    {
        if (!isRecording) return; // 녹음 중이 아니라면 실행 방지

        if (voiceRecorder != null)
        {
            voiceRecorder.StopRecording();
            isRecording = false; // 녹음 상태 업데이트
            Debug.Log("녹음 중지 및 처리 시작!");

            // 녹음된 파일 처리
            voiceRecorder.ConvertAudioToText();
        }
        else
        {
            Debug.LogError("VoiceRecorder가 연결되지 않았습니다!");
        }
    }
}
