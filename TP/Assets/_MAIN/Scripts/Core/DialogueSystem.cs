using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    // 대화 시스템에서 사용하는 대화 데이터를 저장하는 DialogueContainer 객체
    // Inspector에서 수정 가능하도록 하려면 SerializeField 속성을 사용해야 하지만, 현재는 public으로 선언되어 있음
    // [SerializeField] private DialogueContainer dialogueContainer = new DialogueContainer();
    public DialogueContainer dialogueContainer = new DialogueContainer();

    // 싱글톤(Singleton) 패턴을 위한 static 변수
    public static DialogueSystem instance;

    // Unity의 Awake 메서드: 오브젝트 초기화 시 호출됨
    private void Awake()
    {   
        // 싱글톤 패턴 구현
        if (instance == null) // instance가 null인 경우, 현재 오브젝트를 instance로 설정
            instance = this;
        else
            DestroyImmediate(gameObject); // 이미 instance가 존재하면 중복된 오브젝트를 즉시 삭제
    }

    //사용 이유: 다른 스크립트에서 DialogueSystem에 쉽게 접근하고, 대화 시스템이 한 번만 생성되도록 보장합니다.

    // Unity의 Start 메서드: 스크립트 활성화 시 최초 한 번 호출됨
    void Start()
    {
        // 현재 Start 메서드는 구현 내용 없음
    }

    // Unity의 Update 메서드: 매 프레임마다 호출됨
    void Update()
    {
        // 현재 Update 메서드는 구현 내용 없음
    }
}
