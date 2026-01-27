using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// VR 환경에서 사용자의 음성 입력을 녹음하는 클래스
// 녹음된 음성은 STT 처리 단계로 전달됨

public class GoogleTextToSpeech : MonoBehaviour
{
    public string apiKey = ""; // Google API 키
    public AudioSource audioSource; // Unity AudioSource 컴포넌트

    // TTS 변환 함수
    public void ConvertTextToSpeech(string text)
    {
        string url = $"https://texttospeech.googleapis.com/v1/text:synthesize?key={apiKey}";

        // JSON 요청 데이터
        string jsonRequest = "{ " +
            "\"input\": { \"text\": \"" + EscapeJsonString(text) + "\" }, " +
            "\"voice\": { \"languageCode\": \"ko-KR\", \"ssmlGender\": \"FEMALE\" }, " +
            "\"audioConfig\": { \"audioEncoding\": \"LINEAR16\" } " +
            "}";

        StartCoroutine(PostRequest(url, jsonRequest));
    }

    // OpenAI 응답을 TTS로 변환하는 함수
    public void ConvertOpenAIResponseToSpeech(string responseText)
    {
        ConvertTextToSpeech(responseText);
    }

    public void PlayResponse(string responseText)
    {
        ConvertTextToSpeech(responseText); // OpenAI 응답을 TTS로 변환하여 재생
    }

    // TTS 요청을 처리하는 코루틴
    private IEnumerator PostRequest(string url, string json)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("TTS 요청 성공!");

            // 응답 처리
            string jsonResponse = request.downloadHandler.text;
            TTSResponse response = JsonUtility.FromJson<TTSResponse>(jsonResponse);

            if (!string.IsNullOrEmpty(response.audioContent))
            {
                // Base64 데이터를 디코딩하여 오디오 재생
                byte[] audioData = System.Convert.FromBase64String(response.audioContent);
                Debug.Log($"오디오 데이터 길이: {audioData.Length}"); // 디버깅 추가
                PlayAudio(audioData);
            }
            else
            {
                Debug.LogError("TTS 응답에 오디오 데이터가 없습니다.");
            }
        }
        else
        {
            Debug.LogError($"TTS 요청 실패: {request.error}");
        }
    }


    // 오디오 데이터 재생
    private void PlayAudio(byte[] audioData)
    {
        if (audioData == null || audioSource == null)
        {
            Debug.LogError("AudioSource 또는 오디오 데이터가 없습니다.");
            return;
        }

        AudioClip audioClip = WavUtility.ToAudioClip(audioData, 0, "TTS_Audio");
        if (audioClip == null)
        {
            Debug.LogError("AudioClip 변환 실패.");
            return;
        }

        audioSource.clip = audioClip;
        audioSource.Play();
    }

    // 텍스트에서 JSON 문자열로 이스케이프 처리
    private string EscapeJsonString(string text)
    {
        // JSON 문자열에서 필요한 이스케이프 처리
        return text.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    // TTS 응답을 받기 위한 클래스
    [Serializable]
    private class TTSResponse
    {
        public string audioContent; // Base64로 인코딩된 WAV 데이터
    }

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트가 없다면 자동으로 할당
        }
    }
}
