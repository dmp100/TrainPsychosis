using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대화 파일을 테스트하기 위한 클래스입니다.
/// 텍스트 파일을 읽어서 DialogueSystem을 통해 대화를 시작합니다.
/// </summary>
public class TestDialogueFiles : MonoBehaviour
{
    // Unity Inspector에서 할당할 텍스트 파일 에셋
    [SerializeField] private TextAsset fileToRead = null;

    // 오브젝트가 활성화될 때 호출되는 Start 메서드
    void Start()
    {
        StartConversation();
    }

    /// <summary>
    /// 대화를 시작하는 메서드입니다.
    /// 텍스트 파일을 읽고 파싱하여 DialogueSystem으로 전달합니다.
    /// </summary>
    void StartConversation()
    {
        // 텍스트 파일이 할당되었는지 확인
        if (fileToRead == null)
        {
            Debug.LogError("No dialogue file assigned! Please assign a text file in the inspector.");
            return;
        }

        // DialogueSystem 인스턴스가 존재하는지 확인
        if (DialogueSystem.instance == null)
        {
            Debug.LogError("DialogueSystem instance not found! Make sure DialogueSystem is in the scene.");
            return;
        }

        // 파일 내용을 직접 확인하기 위한 디버그 로그
        Debug.Log($"Reading file: {fileToRead.name}");
        Debug.Log($"Raw file content: {fileToRead.text}");

        // FileManagers를 사용하여 텍스트 파일의 내용을 라인별로 읽어옴
        List<string> lines = FileManagers.ReadTextAsset(fileToRead);

        // 읽어온 라인이 있는지 확인
        if (lines == null || lines.Count == 0)
        {
            Debug.LogError("No dialogue lines found in file!");
            return;
        }

        // 디버깅을 위한 각 라인의 파싱 결과 확인
        // 주석 해제하여 사용할 수 있음
        /*
        foreach (string line in lines)
        {
            // 빈 라인은 건너뜀
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // 현재 처리 중인 라인 출력
            Debug.Log($"Processing line: {line}");

            // DialogueParser를 사용하여 라인을 파싱
            DIALOGUE_LINE dl = DialogueParser.Parse(line);

            // 파싱된 대화 내용이 있다면 출력
            if (dl.hasDialogue)
            {
                Debug.Log($"Dialogue found: {dl.dialogueData.segments[0].dialogue}");
            }

            // 명령어가 있다면 각 명령어의 정보를 출력
            if (dl.hasCommands)
            {
                for (int i = 0; i < dl.commandData.commands.Count; i++)
                {
                    DL_COMMAND_DATA.Command command = dl.commandData.commands[i];
                    Debug.Log($"Command [{i}] '{command.name}' has arguments [{string.Join(", ", command.arguments)}]");
                }
            }

            // 인코딩 확인을 위한 바이트 길이 출력
            Debug.Log($"Line encoding check: {System.Text.Encoding.UTF8.GetBytes(line).Length} bytes");
        }
        */

        // DialogueSystem을 통해 대화 시작
        // Say 메서드는 라인 리스트를 받아 순차적으로 대화를 진행함
        DialogueSystem.instance.Say(lines);
    }
}