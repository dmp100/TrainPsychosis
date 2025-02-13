using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 라이브러리 사용 (Unity UI 텍스트 처리)

namespace DIALOGUE
{
    // 대화 시스템을 관리하는 메인 클래스
    public class DialogueSystem : MonoBehaviour
    {
        // 대화 UI 요소들을 포함하는 컨테이너 객체
        public DialogueContainer dialogueContainer = new DialogueContainer();

        // 대화 진행을 관리하는 매니저 객체
        private ConversationManager conversationManager;

        // 텍스트 출력 효과를 담당하는 객체
        private TextArchitect architect;

        // 싱글톤 패턴 구현을 위한 정적 인스턴스
        public static DialogueSystem instance;

        // 사용자 입력에 대한 이벤트 델리게이트 정의
        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserPrompt_Next;

        // 현재 대화가 진행 중인지 확인하는 프로퍼티
        public bool isRunningConversation => conversationManager.isRunning;

        // Unity 생명주기 - Awake: 객체가 처음 생성될 때 호출
        private void Awake()
        {
            // 싱글톤 패턴: 하나의 인스턴스만 유지
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
            else
                DestroyImmediate(gameObject); // 중복 인스턴스 제거
        }

        // 초기화 상태 추적
        bool _initialized = false;

        // 시스템 초기화 메서드
        private void Initialize()
        {
            if (_initialized)
                return;

            Debug.Log($"Initializing DialogueSystem with font: {dialogueContainer.dialogueText.font.name}");
            architect = new TextArchitect(dialogueContainer.dialogueText);
            conversationManager = new ConversationManager(architect);
            _initialized = true;
        }
        // 사용자 입력 처리 메서드
        public void OnUserPrompt_Next()
        {
            onUserPrompt_Next?.Invoke(); // 이벤트 구독자들에게 알림
        }

        // 화자 이름 표시 메서드
        public void ShowSpeakerName(string speakerName = "")
        {
            // 나레이터가 아닌 경우에만 이름 표시
            if (speakerName.ToLower() != "narrator")
                dialogueContainer.nameContainer.Show(speakerName);
            else
                HideSpeakerName();
        }

        // 화자 이름 숨기기 메서드
        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();

        // 단일 대화 시작 메서드
        public void Say(string speaker, string dialogue)
        {
            // 단일 대화를 리스트로 변환하여 처리
            List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
            Say(conversation);
        }

        // 여러 대화 시작 메서드
        public void Say(List<string> conversation)
        {
            conversationManager.StartConversation(conversation);
        }
    }
}