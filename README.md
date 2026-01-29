# 🧠 AI NPC 기반 VR 재난 대피 교육 시스템

## 📌 프로젝트 개요
본 프로젝트는 **VR 환경에서 사용자의 음성 및 행동에 반응하는 AI NPC를 구현**하여  
재난 상황에서의 **실감형 대피 교육**을 제공하는 것을 목표로 한 팀 프로젝트입니다.

POSCO Academy AI Project의 일환으로 진행되었으며,  
기존 VR 안전 교육의 한계였던 **일방향 안내 방식**을 개선하여  
**사용자와 상호작용하는 AI NPC**를 구현했습니다.

- **프로젝트 유형**: 팀 프로젝트 (6명)
- **주제**: VR을 활용한 실감형 재난 대피 교육

---

## 🙋‍♂️ 담당 역할
본 레포지토리는 프로젝트 전체 중 **AI NPC 구현 파트**를 정리한 저장소입니다.

제가 담당한 주요 역할은 다음과 같습니다.

- AI NPC **상호작용 로직 설계 및 구현**
- 사용자 **음성 입력(STT) → 응답 생성(LLM) → 음성 출력(TTS)** 흐름 구현
- 상황(화재 위치, 사용자 위치 등)에 따른 **NPC 안내 분기 처리**
- VR 환경에서 사용자 질문에 실시간으로 응답하는 **양방향 대화 시스템 구현**

---

## 🧩 AI NPC 동작 흐름

사용자 음성 입력  
 → 음성 녹음  
 → STT (음성 → 텍스트)  
 → LLM 기반 응답 생성  
 → TTS (텍스트 → 음성)  
 → NPC 음성 및 행동 출력  

재난 상황 정보(화재 위치, 탈출구 위치 등)를 반영하여  
단순한 일반 안내가 아닌 **상황 기반 안내**를 제공하도록 설계했습니다.

---

## 📂 레포지토리 구조

```text
AI-NPC-VR-Safety-Training  
│
├── AI-NPC/ 
│   ├── NPCInteractionManager.cs # NPC 대화 및 상태 관리  
│   ├── OpenAIChat.cs # LLM 기반 응답 처리  
│   ├── GoogleSpeechToText.cs # 음성 인식(STT)  
│   ├── GoogleTextToSpeech.cs # 음성 합성(TTS)  
│   ├── VoiceRecorder.cs # 사용자 음성 녹음  
│   └── SavWav.cs / WavUtility.cs # 음성 파일 처리 유틸
│
├── Optional-VR-Controls/ 
│   └── VR 이동 및 컨트롤 관련 스크립트
│
├── docs/  
│   └── presentation.pdf # 프로젝트 발표 자료
│
└── README.md

```

---

## 🛠 사용 기술
- **개발 환경**: Unity, C#
- **VR 기기**: Meta Quest 3
- **음성 처리**
  - Google Cloud Speech-to-Text
  - Google Cloud Text-to-Speech
- **대화 처리**
  - LLM(OpenAI API 기반)
  - Prompt Engineering 적용

※ 본 레포지토리에는 **API Key 및 민감 정보가 포함되어 있지 않습니다.**

---

## 🎬 시연 영상
- [비공개 데모 영상 보기 (Google Drive)](https://drive.google.com/file/d/1f8HaxWOFGztVWB4vKKlb7D5inSqZcvWd/view?usp=drive_link)

실제 VR 환경에서 AI NPC와 음성으로 상호작용하며 대피 안내를 받는 과정을 시연합니다.  
※ 본 영상은 포트폴리오 확인 목적에 한해 비공개로 공유됩니다.

---

## 🚧 기술적 고민 및 해결
- **문제**: 일반적인 LLM 응답은 위치·상황을 고려하지 못함  
  → **해결**: 프롬프트에 재난 상황 정보와 맥락을 함께 전달하여 응답 정확도 개선

- **문제**: 실시간 음성 인식 및 응답 지연  
  → **해결**: 음성 처리 파이프라인 분리 및 예외 처리로 안정성 확보

---

## 📎 참고 자료
- 프로젝트 발표 자료: `docs/presentation.pdf`
