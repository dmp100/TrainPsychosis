using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace DIALOGUE
{
    // 대화 진행을 전체적으로 관리하는 클래스
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
            process = dialogueSystem.StartCoroutine(RunningConversation(conversation));
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
        IEnumerator RunningConversation(List<string> conversation)
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
                    yield return Line_RunDialogue(line);

                // 명령어가 있으면 명령어 실행
                if (line.hasCommands)
                    yield return Line_RunCommands(line);

                if (line.hasDialogue)
                    // 사용자 입력 대기
                    yield return WaitForUserInput();
            }
        }

        // 대사 라인 실행 코루틴
        IEnumerator Line_RunDialogue(DIALOGUE_LINE line)
        {
            // 화자가 있으면 화자 이름 표시
            if (line.hasSpeaker)
                dialogueSystem.ShowSpeakerName(line.speakerData.displayname);

            // 대사 세그먼트 구성 및 출력
            yield return BuildLineSegments(line.dialogueData);
        }

        // 명령어 실행 코루틴
        IEnumerator Line_RunCommands(DIALOGUE_LINE line)
        {
            List<DL_COMMAND_DATA.Command> commands = line.commandData.commands;

            foreach (DL_COMMAND_DATA.Command command in commands)
            {
                if (command.waitForCompletion)
                    yield return CommandManager.instance.Execute(command.name, command.arguments);
                else
                    CommandManager.instance.Execute(command.name, command.arguments);
            }

            yield return null;
        }

        // 대화 세그먼트를 순차적으로 구성하는 코루틴
        IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line)
        {
            for (int i = 0; i < line.segments.Count; i++)
            {
                DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment = line.segments[i];

                // 세그먼트 시작 신호 대기
                yield return WaitForDialogueSegmentSignalToBeTriggered(segment);

                // 세그먼트 대사 구성
                yield return BuildDialogue(segment.dialogue, segment.appendText);
            }
        }

        // 세그먼트 시작 신호에 따른 처리를 담당하는 코루틴
        IEnumerator WaitForDialogueSegmentSignalToBeTriggered(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment)
        {
            switch (segment.startSignal)
            {
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.C:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WA:
                    yield return new WaitForSeconds(segment.signalDelay);
                    break;
                default:
                    break;
            }
        }

        // 대사 출력을 담당하는 코루틴
        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {
            // 대사 구성 시작
            if (!append)
                architect.Build(dialogue);
            else
                architect.Append(dialogue);

            // 대사 구성이 완료될 때까지 대기
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
        IEnumerator WaitForUserInput()
        {
            while (!userPrompt)
                yield return null;

            userPrompt = false;
        }
    }
}