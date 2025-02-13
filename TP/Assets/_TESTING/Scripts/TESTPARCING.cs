using DIALOGUE;
using UnityEngine;
using System.Collections.Generic;

namespace TESTING
{
    public class TESTPARCING : MonoBehaviour
    {
        // TextAsset을 Inspector에서 할당할 수 있도록 선언
        [SerializeField] private TextAsset file;

        void Start()
        {
            SendFileToParse();
        }

        void SendFileToParse()
        {
            // file이 할당되었는지 확인
            if (file == null)
            {
                Debug.LogError("대화 파일이 할당되지 않았습니다. Inspector에서 파일을 할당해주세요.");
                return;
            }

            // TextAsset을 직접 전달
            List<string> lines = FileManagers.ReadTextAsset(file);

            // 디버깅을 위한 로그 추가
            Debug.Log($"파일 '{file.name}'에서 {lines.Count}개의 라인을 읽었습니다.");

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                Debug.Log($"파싱 중인 라인: {line}");
                DIALOGUE_LINE dl = DialogueParser.Parse(line);
            }
        }
    }
}