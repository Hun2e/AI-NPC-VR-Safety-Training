# AI-NPC-VR-Safety-Training

> VR 화재 대피 교육에서 **음성으로 묻고 답하는 AI NPC**

![Unity](https://img.shields.io/badge/Unity-000000?style=flat-square&logo=unity&logoColor=white) ![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=csharp&logoColor=white) ![OpenAI](https://img.shields.io/badge/OpenAI_API-412991?style=flat-square&logo=openai&logoColor=white) ![Google Cloud](https://img.shields.io/badge/Google_Cloud_STT·TTS-4285F4?style=flat-square&logo=googlecloud&logoColor=white) ![Meta Quest](https://img.shields.io/badge/Meta_Quest_3-0467DF?style=flat-square&logo=meta&logoColor=white)

<sub>POSCO Academy AI 프로젝트 · 팀 6인 · AI NPC 파트 담당</sub>

<br/>

## 한눈에 보기

| 항목 | 내용 |
|---|---|
| 문제 | 기존 VR 안전 교육은 일방향 안내라 사용자가 질문할 수 없음 |
| 해결 | 사용자의 음성 질문에 NPC가 음성으로 답함 |
| 내 역할 | AI NPC 상호작용 로직, STT → LLM → TTS 음성 파이프라인 |
| 데모 | [시연 영상 (Google Drive, 비공개 공유)](https://drive.google.com/file/d/1f8HaxWOFGztVWB4vKKlb7D5inSqZcvWd/view?usp=drive_link) |

<br/>

## 동작 흐름

```mermaid
flowchart LR
  V["음성 녹음<br/>VoiceRecorder"] --> S["STT<br/>Google Cloud"]
  S --> L["응답 생성<br/>OpenAI Chat"]
  L --> T["TTS<br/>Google Cloud"]
  T --> N["NPC 음성 응답<br/>NPCInteractionManager"]
```

<br/>

## 주요 구현

- **음성 대화 파이프라인** — 녹음 → STT → LLM → TTS를 스크립트별로 나누고, 단계마다 실패 시 로그를 남깁니다.
- **시나리오 맞춤 프롬프트** — 소화기 위치, 사람이 몰린 구역, 가장 안전한 대피 경로를 시스템 프롬프트에 넣어 화재 상황에 맞는 답을 하도록 했습니다.
- **동행하는 NPC** — NPC가 사용자 곁을 따라다니며 언제든 질문할 수 있게 했습니다.

<br/>

## 코드 구성

| 파일 | 역할 |
|---|---|
| [`NPCInteractionManager.cs`](AI-NPC/NPCInteractionManager.cs) | 대화 흐름과 NPC 상태 관리 |
| [`OpenAIChat.cs`](AI-NPC/OpenAIChat.cs) | LLM 요청·응답 처리 |
| [`GoogleSpeechToText.cs`](AI-NPC/GoogleSpeechToText.cs) · [`GoogleTextToSpeech.cs`](AI-NPC/GoogleTextToSpeech.cs) | 음성 인식 · 합성 |
| [`VoiceRecorder.cs`](AI-NPC/VoiceRecorder.cs) · `SavWav.cs` · `WavUtility.cs` | 녹음과 WAV 변환 |
| [`NPCFollower.cs`](AI-NPC/NPCFollower.cs) | NPC가 사용자를 따라 이동 |
| [`Optional-VR-Controls/`](Optional-VR-Controls) | VR 이동·컨트롤 (참고용) |

API 키는 저장소에 없으며 Unity Inspector에서 입력합니다. 발표 자료는 [`docs/presentation.pdf`](docs/presentation.pdf)에 있습니다.
