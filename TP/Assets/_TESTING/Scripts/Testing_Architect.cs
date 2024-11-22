using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class Testing_Architect : MonoBehaviour
    {
        // DialogueSystem 싱글톤 인스턴스 참조
        //DialogueSystem 싱글톤
            //DialogueSystem.instance를 통해 전역에서 동일한 인스턴스를 공유합니다.
            //dialogueContainer를 통해 텍스트 출력 컴포넌트를 가져옵니다.
            
        DialogueSystem ds;
        // TextArchitect 객체: 텍스트를 출력하는 데 사용
        TextArchitect architect;

        // 랜덤으로 출력될 텍스트 배열
        string[] lines = new string[5]
        {
            "개씨발",
            "싱글턴은 왜쓰는거임",
            "어머련",
            "결속",
            "밴드 병@신새끼야"
        };

        // Start 메서드: 스크립트가 활성화될 때 한 번 실행
        void Start()
        {
            // DialogueSystem의 싱글톤 인스턴스 가져오기
            ds = DialogueSystem.instance;
            // TextArchitect 객체 초기화, DialogueContainer의 dialogueText 사용
            architect = new TextArchitect(ds.dialogueContainer.dialogueText);
            // 텍스트 출력 방식을 타자 효과(typewriter)로 설정
            architect.buildMethod = TextArchitect.BuildMethod.typewriter;
            // 텍스트 출력 속도 설정
            architect.speed = 0.5f;
        }

        // Update 메서드: 매 프레임 호출
        void Update()
        {
            // 매우 긴 문자열 (Append 예제용)
            string LongLine = "애미씨발좆같은병신버러지새끼가이재명처럼형보수지당하고싶어서입을는순간에겐고처럼자지아가리에물려서게이화시켜버린다.";

            // 스페이스바 입력 처리
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (architect.isBuilding) // 현재 텍스트 출력 중인지 확인
                {
                    if (!architect.hurryup) // hurryup 설정이 안 되어 있으면 설정
                        architect.hurryup = true;
                    else
                        architect.ForceComplete(); // hurryup 상태에서 강제로 출력 완료
                }
                else
                {
                    // 랜덤으로 lines 배열 중 하나를 선택해 출력
                    architect.Build(lines[Random.Range(0, lines.Length)]);
                }
            }
            else if (Input.GetKeyDown(KeyCode.A)) // A키 입력 처리
            {
                // LongLine을 텍스트에 추가하여 출력
                architect.Append(LongLine);
                // architect.Append(lines[Random.Range(0, lines.Length)]); // lines 배열 중 랜덤 추가 가능
            }
        }
    }
}
