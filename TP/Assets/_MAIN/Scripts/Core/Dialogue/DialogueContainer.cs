using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// nameText를 직접 참조하는 대신 NameContainer 타입의 객체를 사용합니다

namespace DIALOGUE
{
    [System.Serializable]
    public class DialogueContainer
    {
        [SerializeField] public GameObject root;
        [SerializeField] public NameContainer nameContainer = new NameContainer();  // NameContainer 추가
        [SerializeField] public TextMeshProUGUI dialogueText;
    }
}




//namespace DIALOGUE
//{
//    [System.Serializable] // 이 클래스를 Unity에서 직렬화 가능하도록 지정
//    public class DialogueContainer
//    {
//        public GameObject root;
//        // 대화 UI의 루트 오브젝트를 참조
//        // (예: 대화창 전체 패널을 비활성화/활성화하기 위해 사용)

//        public TextMeshProUGUI nameText;
//        // 대화 UI에서 캐릭터 이름을 표시하는 TextMeshProUGUI 컴포넌트를 참조

//        public TextMeshProUGUI dialogueText;
//        // 대화 UI에서 대화 내용을 표시하는 TextMeshProUGUI 컴포넌트를 참조
//    }
//}