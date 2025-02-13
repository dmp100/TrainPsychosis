using DIALOGUE;
using UnityEngine;
using System.Collections.Generic;

public class testConversations : MonoBehaviour
{
    // TextAsset을 Inspector에서 할당할 수 있도록 선언
    [SerializeField] private TextAsset dialogueFile;

    void Start()
    {
        StartConversation();
    }

    void StartConversation()
    {
        // dialogueFile이 할당되었는지 확인
        if (dialogueFile == null)
        {
            Debug.LogError("대화 파일이 할당되지 않았습니다. Inspector에서 파일을 할당해주세요.");
            return;
        }

        // DialogueSystem이 존재하는지 확인
        if (DialogueSystem.instance == null)
        {
            Debug.LogError("DialogueSystem이 씬에 존재하지 않습니다.");
            return;
        }

        // TextAsset을 직접 전달
        List<string> lines = FileManagers.ReadTextAsset(dialogueFile);

        // 디버깅을 위한 상세 로그
        Debug.Log($"파일 '{dialogueFile.name}'에서 {lines.Count}개의 라인을 읽었습니다.");
        foreach (string line in lines)
        {
            // 각 라인의 바이트 정보도 출력하여 인코딩 확인
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(line);
            Debug.Log($"읽은 라인: {line}");
            Debug.Log($"라인의 바이트 정보: {System.BitConverter.ToString(bytes)}");
        }

        // 대화 시스템에 라인 전달
        DialogueSystem.instance.Say(lines);
    }
}