using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

public class GoogleSpeechToText : MonoBehaviour
{
    public string apiKey = ""; // Google STT API 키
    

    public void ConvertSpeechToText(string filePath, System.Action<string> callback)
    {
        StartCoroutine(ConvertSpeechToTextCoroutine(filePath, callback));
    }

    private IEnumerator ConvertSpeechToTextCoroutine(string filePath, System.Action<string> callback)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"파일이 존재하지 않습니다: {filePath}");
            callback(null);
            yield break;
        }

        byte[] audioData = File.ReadAllBytes(filePath);

        // v2 엔드포인트 사용
        string url = $"https://speech.googleapis.com/v1/speech:recognize?key={apiKey}";
        string jsonRequest = $@"
        {{
            ""config"": {{
                ""encoding"": ""LINEAR16"",
                ""sampleRateHertz"": 16000,
                ""languageCode"": ""ko-KR""
            }},
            ""audio"": {{
                ""content"": ""{System.Convert.ToBase64String(audioData)}""
            }}
        }}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonRequest);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("STT 요청 전송 중...");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("STT 요청 성공!");
            string response = request.downloadHandler.text;
            Debug.Log($"STT 응답: {response}");

            try
            {
                string transcript = ParseSTTResponse(response);
                // 텍스트 파일 저장
                string textFilePath = Path.Combine(Application.persistentDataPath, "STTResult.txt");
                SaveTextToFile(transcript, textFilePath);
                callback(transcript);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"STT 응답 처리 중 오류: {ex.Message}");
                callback(null);
            }
        }
        else
        {
            Debug.LogError($"STT 실패: {request.error}");
            Debug.LogError($"상세 응답: {request.downloadHandler.text}");
            callback(null);
        }
    }

    private void SaveTextToFile(string text, string filePath)
    {
        try
        {
            File.WriteAllText(filePath, text);
            Debug.Log($"텍스트 저장 성공: {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"텍스트 저장 실패: {ex.Message}");
        }
    }


    private string ParseSTTResponse(string jsonResponse)
    {
        try
        {
            var parsed = JsonUtility.FromJson<STTResponse>(jsonResponse);
            if (parsed.results != null && parsed.results.Length > 0 &&
                parsed.results[0].alternatives != null && parsed.results[0].alternatives.Length > 0)
            {
                return parsed.results[0].alternatives[0].transcript;
            }
            else
            {
                Debug.LogError("STT 응답에 결과가 없습니다.");
                return null;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"STT JSON 파싱 오류: {ex.Message}");
            throw;
        }
    }


    [System.Serializable]
    private class STTResponse
    {
        public Result[] results;
    }

    [System.Serializable]
    private class Result
    {
        public Alternative[] alternatives;
    }

    [System.Serializable]
    private class Alternative
    {
        public string transcript;
    }
}
