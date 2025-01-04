using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DIALOGUE

{
    public class DialogueSystem : MonoBehaviour
    {
        // 대화 시스템에서 사용하는 대화 데이터를 저장하는 DialogueContainer 객체
        // Inspector에서 수정 가능하도록 하려면 SerializeField 속성을 사용해야 하지만, 현재는 public으로 선언되어 있음
        // [SerializeField] private DialogueContainer dialogueContainer = new DialogueContainer();
        
        public DialogueContainer dialogueContainer = new DialogueContainer();
        private ConversationManager conversationManager;
        private TextArchitect architect;
        

        // 싱글톤(Singleton) 패턴을 위한 static 변수
        public static DialogueSystem instance;


        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserPrompt_Next;


        public bool isRunningConversation => conversationManager.isRunning;

        // Unity의 Awake 메서드: 오브젝트 초기화 시 호출됨
        private void Awake()
        {
            // 싱글톤 패턴 구현
            if (instance == null) // instance가 null인 경우, 현재 오브젝트를 instance로 설정
            {
                instance = this;
                Initialize();
            }
            else
                DestroyImmediate(gameObject); // 이미 instance가 존재하면 중복된 오브젝트를 즉시 삭제
        
        }

        bool _initialized = false;

        private void Initialize()
        {
            if(_initialized) 
                return;

            architect = new TextArchitect(dialogueContainer.dialogueText);

            conversationManager = new ConversationManager(architect);

        }


        public void OnUserPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();
        }

        public void ShowSpeakerName(string speakerName = "")
        {

            if(speakerName.ToLower() != "narrator")
            dialogueContainer.nameContainer.Show(speakerName);
            else
                // 나레이터 이름 제거
                HideSpeakerName();
        }

        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();

        //사용 이유: 다른 스크립트에서 DialogueSystem에 쉽게 접근하고, 대화 시스템이 한 번만 생성되도록 보장합니다.

        public void Say(string speaker, string dialogue)
        {
            List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
            Say(conversation);

        }

        public void Say(List<string> conversation)
        {
            conversationManager.StartConversation(conversation);
        }

    }
}