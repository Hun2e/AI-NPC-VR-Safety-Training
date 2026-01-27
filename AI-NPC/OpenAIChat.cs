using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using System.IO;

public class OpenAIChat : MonoBehaviour
{
    public string apiKey = ""; // OpenAI API 키
    public TMP_InputField userInput; // InputField 연결
    public TextMeshProUGUI responseText; // OpenAI 응답 표시할 TextMeshPro - Text 연결

    // 기존 사용자 입력 텍스트 기반 OpenAI 요청 메서드
    public void SendMessageToOpenAI()
    {
        if (userInput == null || responseText == null)
        {
            Debug.LogError("InputField 또는 ResponseText가 연결되지 않았습니다!");
            return;
        }

        string userMessage = userInput.text.Trim();

        if (string.IsNullOrEmpty(userMessage))
        {
            responseText.text = "입력한 메시지가 없습니다. 텍스트를 입력하세요.";
            Debug.LogWarning("InputField가 비어 있습니다.");
            return;
        }

        responseText.text = "OpenAI에 요청 중..."; // 요청 상태 표시
        StartCoroutine(SendRequest(userMessage));
    }

    // 텍스트 파일의 내용을 OpenAI로 전송
    public void ProcessTextFile(string filePath, System.Action<string> callback)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"텍스트 파일이 존재하지 않습니다: {filePath}");
            callback(null);
            return;
        }

        string text = File.ReadAllText(filePath);
        SendMessageToOpenAI(text, callback);
    }

    // 오버로드된 메서드: 사용자 메시지와 콜백 처리
    public void SendMessageToOpenAI(string userMessage, System.Action<string> callback)
    {
        if (string.IsNullOrEmpty(userMessage))
        {
            Debug.LogError("입력 메시지가 비어 있습니다.");
            callback(null);
            return;
        }

        StartCoroutine(SendRequestWithCallback(userMessage, callback));
    }

    // OpenAI 요청 및 응답 처리
    private IEnumerator SendRequest(string userMessage)
    {
        string url = "https://api.openai.com/v1/chat/completions";

        var messages = new[]
        {
            new
        {
            role = "system",
            content = "당신은 화재 대피에 대한 안내를 하는 친절한 안내원입니다."
                    + "\"소화기 어디에 있어?\"라는 질문을 받으면 \"왼쪽 화장실 입구에 소화기가 있어요! 진화 실패 시 신속히 대피하세요!\"라는 답변 생성 "
        },
        //     content = "당신은 화재 대피에 대한 안내를 하는 친절한 안내원입니다.  "
        //             + "장소는 인공지능 연구원입니다. 그리고 플레이어의 현재 위치는 인공지능 연구원 221호입니다. "
        //             + "현재 화재 발생 위치는 수학분야 연구실 앞 복도에 발생했습니다. "
        //             + "\"어디로 대피해야 해?\"라는 질문을 받으면"
        //             + "\"현재 엘레베이터 옆 계단에 사람이 몰려있어 정문으로 대피하는걸 권장합니다!\" 라는 답변 생성."
        // },
            new { role = "user", content = userMessage }
        };

        var requestData = new
        {
            model = "gpt-4",
            messages = messages,
            max_tokens = 150,
            temperature = 0.7
        };

        string jsonBody = JsonConvert.SerializeObject(requestData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        Debug.Log($"OpenAI API 요청: {userMessage}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("OpenAI 응답 성공!");

            // OpenAI 응답 처리
            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"응답 JSON: {jsonResponse}");

            try
            {
                var response = JsonConvert.DeserializeObject<OpenAIResponse>(jsonResponse);
                string reply = response.choices[0].message.content.Trim();

                responseText.text = reply; // 응답 표시
                Debug.Log($"OpenAI 응답 메시지: {reply}");
            }
            catch (System.Exception ex)
            {
                responseText.text = "응답 처리 중 오류가 발생했습니다.";
                Debug.LogError($"JSON 파싱 오류: {ex.Message}");
            }
        }
        else
        {
            responseText.text = "OpenAI 요청에 실패했습니다.";
            Debug.LogError($"OpenAI 요청 실패: {request.error}");
            Debug.LogError($"상세 응답: {request.downloadHandler.text}");
        }
    }

    // OpenAI 요청 및 콜백 처리
    private IEnumerator SendRequestWithCallback(string userMessage, System.Action<string> callback)
    {
        string url = "https://api.openai.com/v1/chat/completions";

        var messages = new[]
        {
            new { role = "system", 
            content = "자기소개 단어 포함 질문 시에만 \"궁금하신게 있으면 언제든지 물어봐주세요!\"라는 말을 포함해서 답변합니다." 
                        +"현재 플레이어는 화재 대피 상황으로 대피하는 중입니다."
                        +"플레이어가 \"여기로 가는게 맞아?\"라는 말이 포함된 질문을 할 경우 \"아니요, 화면에 표시되는 화살표를 따라가야 가장 안전하고 빠른 대피경로입니다.\"를 답변합니다."
                        +"플레이어가 \"나 따라해\"라는 말을 포함해서 질문할 경우 플레이어의 \"나 따라해\"를 제외한 뒤의 말을 따라하세요"
                        +"플레어가 소화기라는 단어를 포함한 질문할 경우 \"왼쪽 화장실 입구에 소화기가 있어요!\"라는 답변을 합니다."
                        +"플레이어가 불이 안꺼진다는 질문을 할 경우 \"불이 너무 거세여서 화재를 진압할 수 없습니다! 소화기를 두고 신속히 대피하세요!\"를 답변합니다."
                        +"**2층 계단과 1층 엘레베이터**는 **사람이 밀집**되어 있어 위험하므로 피하세요."
                        +"가장 안전한 대피 경로는 **1층 정문**입니다."
            },
            
            new { role = "user", content = userMessage }
        };

        var requestData = new
        {
            model = "gpt-4",
            messages = messages,
            max_tokens = 150,
            temperature = 0.7
        };

        string jsonBody = JsonConvert.SerializeObject(requestData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                string jsonResponse = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<OpenAIResponse>(jsonResponse);
                string reply = response.choices[0].message.content.Trim();

                Debug.Log($"OpenAI 응답 성공: {reply}");
                callback(reply);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"JSON 파싱 오류: {ex.Message}");
                callback(null);
            }
        }
        else
        {
            Debug.LogError($"OpenAI 요청 실패: {request.error}");
            callback(null);
        }
    }

    [System.Serializable]
    private class OpenAIResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    [System.Serializable]
    private class Message
    {
        public string content;
    }
}
