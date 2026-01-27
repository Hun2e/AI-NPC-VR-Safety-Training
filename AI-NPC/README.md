## 📄 주요 코드 설명 (AI NPC)

본 프로젝트에서 AI NPC 구현을 위해 작성한 핵심 스크립트들입니다.

- **NPCInteractionManager.cs**  
  AI NPC의 전체 상호작용 흐름을 관리하는 핵심 클래스입니다.  
  사용자 음성 입력을 기반으로 NPC의 대화, 상태 전환, 응답 출력을 제어합니다.

- **OpenAIChat.cs**  
  사용자의 질문을 LLM(OpenAI API)에 전달하고,  
  재난 상황 정보를 포함한 응답을 생성하는 역할을 담당합니다.

- **GoogleSpeechToText.cs**  
  VR 환경에서 녹음된 사용자 음성을 텍스트로 변환하는  
  STT(Speech-to-Text) 처리 로직을 구현한 클래스입니다.

- **GoogleTextToSpeech.cs**  
  AI NPC의 응답 텍스트를 음성으로 변환하여  
  실제 NPC 음성 안내로 출력하는 TTS(Text-to-Speech) 처리 클래스입니다.

- **VoiceRecorder.cs**  
  VR 환경에서 사용자의 음성 입력을 녹음하고,  
  음성 데이터를 STT 처리 단계로 전달하는 역할을 합니다.

- **SavWav.cs / WavUtility.cs**  
  음성 데이터를 WAV 파일 형식으로 변환 및 처리하기 위한  
  유틸리티 클래스입니다.
