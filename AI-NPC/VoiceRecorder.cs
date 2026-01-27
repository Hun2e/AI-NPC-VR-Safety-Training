using UnityEngine;
using TMPro;
using System.IO;

public class VoiceRecorder : MonoBehaviour
{
    private AudioClip audioClip;
    private string microphone;
    private string audioFilePath;
    private bool isRecording = false;

    public GoogleSpeechToText googleSpeechToText;
    public OpenAIChat openAIChat;
    public GoogleTextToSpeech googleTextToSpeech;

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            microphone = Microphone.devices[0];
            audioFilePath = Path.Combine(Application.persistentDataPath, "recordedAudio.wav");
            Debug.Log($"마이크 설정 완료: {microphone}");
        }
        else
        {
            Debug.LogError("사용 가능한 마이크가 없습니다!");
        }
    }

    public void StartRecording()
    {
        if (!isRecording && microphone != null)
        {
            Debug.Log("VoiceRecorder - StartRecording 호출");
            audioClip = Microphone.Start(microphone, true, 300, 16000);
            isRecording = true;
            Debug.Log("녹음 시작");
        }
        else
        {
            Debug.LogWarning("이미 녹음 중이거나 마이크가 설정되지 않았습니다.");
        }
    }

    public void StopRecording()
    {
        if (isRecording && Microphone.IsRecording(microphone))
        {
            Debug.Log("VoiceRecorder - StopRecording 호출");
            int position = Microphone.GetPosition(microphone);
            if (position > 0)
            {
                Microphone.End(microphone);
                isRecording = false;
                Debug.Log("녹음 중지");
                SaveAudioClip(position);
            }
            else
            {
                Debug.LogError("녹음된 데이터가 없습니다!");
                isRecording = false;
            }
        }
        else
        {
            Debug.LogWarning("녹음 중이 아니거나 마이크가 설정되지 않았습니다.");
        }
    }


    private void SaveAudioClip(int position)
    {
        float[] samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);

        float[] trimmedSamples = new float[position * audioClip.channels];
        System.Array.Copy(samples, 0, trimmedSamples, 0, trimmedSamples.Length);

        AudioClip trimmedClip = AudioClip.Create(audioClip.name, position, audioClip.channels, audioClip.frequency, false);
        trimmedClip.SetData(trimmedSamples, 0);

        if (SavWav.Save(audioFilePath, trimmedClip))
        {
            Debug.Log($"오디오 파일 저장 완료: {audioFilePath}");
        }
        else
        {
            Debug.LogError("오디오 파일 저장 실패");
        }
    }

    // ConvertAudioToText를 public으로 수정
    public void ConvertAudioToText()
    {
        if (googleSpeechToText == null || openAIChat == null || googleTextToSpeech == null)
        {
            Debug.LogError("필수 컴포넌트가 연결되지 않았습니다!");
            return;
        }

        if (string.IsNullOrEmpty(audioFilePath))
        {
            Debug.LogError("audioFilePath가 비어 있습니다!");
            return;
        }

        if (!File.Exists(audioFilePath))
        {
            Debug.LogError($"녹음 파일을 찾을 수 없습니다: {audioFilePath}");
            return;
        }

        googleSpeechToText.ConvertSpeechToText(audioFilePath, (sttResult) =>
        {
            if (!string.IsNullOrEmpty(sttResult))
            {
                Debug.Log($"STT 변환 성공: {sttResult}");

                // STT 결과 저장
                string sttFilePath = Path.Combine(Application.persistentDataPath, "STTResult.txt");
                File.WriteAllText(sttFilePath, sttResult);
                Debug.Log($"STT 텍스트 저장 성공: {sttFilePath}");

                openAIChat.ProcessTextFile(sttFilePath, (openAIResponse) =>
                {
                    if (!string.IsNullOrEmpty(openAIResponse))
                    {
                        // OpenAI 응답 저장
                        string responseFilePath = Path.Combine(Application.persistentDataPath, "OpenAIResponse.txt");
                        File.WriteAllText(responseFilePath, openAIResponse);
                        Debug.Log($"OpenAI 응답 텍스트 저장 성공: {responseFilePath}");

                        // TTS 변환 및 재생
                        googleTextToSpeech.ConvertOpenAIResponseToSpeech(openAIResponse);
                    }
                    else
                    {
                        Debug.LogError("OpenAI 응답 실패");
                    }
                });
            }
            else
            {
                Debug.LogError("STT 변환 실패");
            }
        });
    }
}
