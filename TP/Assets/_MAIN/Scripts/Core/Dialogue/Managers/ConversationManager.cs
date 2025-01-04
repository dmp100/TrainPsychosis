using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    // 대화 진행을 관리하는 클래스
    public class ConversationManager
    {
        // DialogueSystem 인스턴스에 대한 참조를 가져오는 프로퍼티
        private DialogueSystem dialogueSystem => DialogueSystem.instance;

        // 현재 실행 중인 대화 프로세스(코루틴)
        private Coroutine process = null;

        // 대화가 진행 중인지 확인하는 프로퍼티 
        public bool isRunning => process != null;

        // 텍스트 출력을 담당하는 TextArchitect 참조
        private TextArchitect architect = null;

        // 사용자 입력 상태 추적
        private bool userPrompt = false;

        // 생성자: TextArchitect를 받아 초기화하고 이벤트 구독
        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;
        }

        // 사용자 입력 처리 메서드
        private void OnUserPrompt_Next()
        {
            userPrompt = true;
        }

        // 대화 시작 메서드
        public void StartConversation(List<string> conversation)
        {
            StopConversation(); // 이전 대화가 있다면 중지
            process = dialogueSystem.StartCoroutine(RunningCoversation(conversation));
        }

        // 진행 중인 대화 중지 메서드
        public void StopConversation()
        {
            if (!isRunning)
                return;
            dialogueSystem.StopCoroutine(process);
            process = null;
        }

        // 대화 진행을 처리하는 코루틴
        IEnumerator RunningCoversation(List<string> conversation)
        {
            for (int i = 0; i < conversation.Count; i++)
            {
                // 빈 라인은 건너뛰기
                if (string.IsNullOrWhiteSpace(conversation[i]))
                    continue;

                // 대화 라인 파싱
                DIALOGUE_LINE line = DialogueParser.Parse(conversation[i]);

                // 대사가 있으면 대사 실행
                if (line.hasDialogue)
                    yield return Line_Rundialogue(line);

                // 명령어가 있으면 명령어 실행
                if (line.hasCommands)
                    yield return Line_RunCommands(line);
            }
        }

        // 대사 라인 실행 코루틴
        IEnumerator Line_Rundialogue(DIALOGUE_LINE line)
        {
            // 화자가 있으면 화자 이름 표시
            if (line.hasSpeaker)
                dialogueSystem.ShowSpeakerName(line.speaker);

            // 대사 출력
            yield return BuildDialogue(line.dialogue);

            // 사용자 입력 대기
            yield return waitForUserInput();
        }

        // 명령어 실행 코루틴
        IEnumerator Line_RunCommands(DIALOGUE_LINE line)
        {
            Debug.Log(line.commands);
            yield return null;
        }

        // 대사 출력 처리 코루틴
        IEnumerator BuildDialogue(string dialogue)
        {
            architect.Build(dialogue);
            while (architect.isBuilding)
            {
                if (userPrompt)
                {
                    // 사용자 입력에 따른 텍스트 출력 속도 조절
                    if (!architect.hurryup)
                        architect.hurryup = true;
                    else
                        architect.ForceComplete();
                    userPrompt = false;
                }
                yield return null;
            }
        }

        // 사용자 입력 대기 코루틴
        IEnumerator waitForUserInput()
        {
            while (!userPrompt)
                yield return null;
            userPrompt = false;
        }
    }
}