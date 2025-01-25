using System.Collections;
using UnityEngine;
using TMPro;
using UnityEditor;
using Unity.VisualScripting;

// TextArchitect: TextMeshPro(TMP) 컴포넌트에 텍스트를 동적으로 출력하는 기능 (예: 타자 효과, 페이드 효과 등)을 구현하는 클래스
public class TextArchitect 
{
    // UI에 사용하는 TextMeshProUGUI 컴포넌트 참조
    private TextMeshProUGUI tmpro_ui;
    // 월드 공간에서 사용하는 TextMeshPro 컴포넌트 참조
    private TextMeshPro tmpro_world;
    // 현재 활성화된 TextMeshPro 객체 반환 (UI 또는 월드)
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    // TMP 컴포넌트에 현재 표시되고 있는 텍스트
    public string currentText => tmpro.text;
    // 효과를 통해 출력될 최종 목표 텍스트
    public string targetText { get; private set; } = "";
    // 기존에 TMP에 있던 텍스트 (새 텍스트 추가 전의 상태)
    public string preText { get; private set; } = "";
    // 기존 텍스트(preText)의 길이
    private int preTextLength = 0;

    // 기존 텍스트와 목표 텍스트를 합친 전체 텍스트
    public string fullTargetText => preText + targetText;

    // 텍스트 출력 방식(즉시 출력, 타자 효과, 페이드 효과)을 정의하는 열거형
    public enum BuildMethod { instant, typewriter, fade }
    // 현재 선택된 텍스트 출력 방식
    public BuildMethod buildMethod = BuildMethod.typewriter;

    // TMP 텍스트의 색상 설정 및 반환
    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }
    
    // 텍스트 출력 속도 반환 및 설정 (기본 속도에 배속 곱)
    public float speed { get { return baseSpeed * speedMulitplier; } set { speedMulitplier = value; } }
    private const float baseSpeed = 1; // 기본 속도
    private float speedMulitplier = 1; // 배속 곱

    // 속도에 따라 한 번에 출력되는 문자 수 결정
    public int charactersPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    private int characterMultiplier = 1; // 기본 문자 수

    // 텍스트 출력이 빠르게 진행되도록 설정 (true일 경우 속도 증가)
    public bool hurryup = false;

    // UI용 TextMeshProUGUI를 사용하는 생성자
    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }

    // 월드 공간용 TextMeshPro를 사용하는 생성자
    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }

    // 텍스트 출력 작업 시작
    public Coroutine Build(string text)
    {
        preText = ""; // 기존 텍스트 초기화
        targetText = text; // 목표 텍스트 설정

        Stop(); // 기존 진행 중이던 작업 중지

        buildProcess = tmpro.StartCoroutine(Building()); // 새로운 출력 작업 시작
        return buildProcess;
    }

    // 기존 텍스트에 새로운 텍스트를 추가하여 출력 작업 시작
    public Coroutine Append(string text)
    {
        preText = tmpro.text; // 기존 텍스트 저장
        targetText = text; // 추가될 목표 텍스트 설정

        Stop(); // 기존 진행 중이던 작업 중지

        buildProcess = tmpro.StartCoroutine(Building()); // 새로운 출력 작업 시작
        return buildProcess;
    }

    private Coroutine buildProcess = null; // 현재 진행 중인 출력 작업
    public bool isBuilding => buildProcess != null; // 출력 작업 진행 여부 반환

    // 출력 작업 중지
    public void Stop()
    {
        if (!isBuilding)
            return; // 진행 중인 작업이 없으면 아무 작업도 하지 않음

        tmpro.StopCoroutine(buildProcess); // 진행 중이던 Coroutine 중지
        buildProcess = null; // 현재 작업 초기화
    }

    // 텍스트를 출력하는 Coroutine (방식에 따라 다르게 동작)
    IEnumerator Building()
    {
        Prepare(); // 출력 방식을 위한 초기 준비
        
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                yield return Build_Typerwriter(); // 타자 효과 방식 출력
                break;
            case BuildMethod.fade:
                yield return Build_Fade(); // 페이드 효과 방식 출력
                break;
        }
    }

    // 출력 작업 완료 시 호출
    private void OnComplete()
    {
        buildProcess = null; // 진행 중인 작업 초기화
        hurryup = false; // 빠른 진행 설정 해제
    }

    // 강제로 텍스트 출력 완료
    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount; // 모든 문자를 한 번에 표시
                break;
            case BuildMethod.fade:
                tmpro.ForceMeshUpdate(); // 더블, 트리플 클릭시 빠르게 나옴
                break;
        }
        Stop(); // 작업 중지
        OnComplete(); // 완료 처리
    }

    // 텍스트 출력 방식을 위한 준비 단계
    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.instant:
                Prepare_Instant(); // 즉시 출력 준비
                break;
            case BuildMethod.typewriter:
                Prepare_Typewriter(); // 타자 효과 준비
                break;
            case BuildMethod.fade:
                Prepare_Fade(); // 페이드 효과 준비
                break;
        }
    }

    // 즉시 출력 방식 준비
    private void Prepare_Instant()
    {
        tmpro.color = tmpro.color; // 기존 색상 유지
        tmpro.text = fullTargetText; // 전체 텍스트를 즉시 설정
        tmpro.ForceMeshUpdate(); // TMP 메쉬 업데이트
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount; // 모든 문자 표시
    }

    // 타자 효과 방식 준비
    private void Prepare_Typewriter()
    {
        tmpro.color = tmpro.color; // 기존 색상 유지
        tmpro.maxVisibleCharacters = 0; // 처음에는 아무 문자도 표시하지 않음
        tmpro.text = preText; // 기존 텍스트 설정

        if (preText != "")
        {
            tmpro.ForceMeshUpdate(); // TMP 메쉬 업데이트
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount; // 기존 텍스트만 표시
        }
        tmpro.text += targetText; // 목표 텍스트 추가
        tmpro.ForceMeshUpdate(); // TMP 메쉬 업데이트
    }

    // 페이드 효과 방식 준비 
    private void Prepare_Fade()
    {
        tmpro.text = preText;
        if (preText != "")
        {
            tmpro.ForceMeshUpdate();
            preTextLength = tmpro.textInfo.characterCount;
        }
        else 
            preTextLength = 0;

        tmpro.text += targetText;
        tmpro.maxVisibleCharacters = int.MaxValue;
        tmpro.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpro.textInfo;

        Color colorVisable = new Color(textColor.r, textColor.g, textColor.b,1);
        Color colorHidden = new Color(textColor.r, textColor.g, textColor.b, 0);

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;

        for(int i=0; i<textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            if (i< preTextLength)
                for(int v = 0; v<4; v++) 
                {
                    vertexColors[charInfo.vertexIndex + v] = colorVisable;
                }
                else
                {
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorHidden;
                }
        }

        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

    }

    // 타자 효과 출력 방식
    private IEnumerator Build_Typerwriter()
    {
        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += hurryup ? charactersPerCycle * 5 : charactersPerCycle; // 빠른 출력 여부에 따라 표시 문자 수 조정
            yield return new WaitForSeconds(0.015f / speed); // 속도에 따른 대기 시간
        }
    }

    // 페이드 효과 출력 방식
    private IEnumerator Build_Fade()
    {
        int minRange = preTextLength;
        int maxRange = minRange + 1;

        byte alphaThreshold = 15;

        TMP_TextInfo textInfo = tmpro.textInfo;

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];

        while (true)
        {
            float fadeSpeed = (hurryup ? charactersPerCycle * 5 : charactersPerCycle * speed) * 4f;

            for (int i = minRange; i < maxRange; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                    continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                alphas[i] = Mathf.MoveTowards(alphas[i], 255, fadeSpeed);

                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v].a = (byte)alphas[i];

                if (alphas[i] >= 255)
                    minRange++;
            }

            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            bool lastCharacterIsInvisible = !textInfo.characterInfo[maxRange - 1].isVisible;
            if (alphas[maxRange - 1] > alphaThreshold || lastCharacterIsInvisible)
            {
                if (maxRange < textInfo.characterCount)
                    maxRange++;
                else if (alphas[maxRange - 1] >= 255 || lastCharacterIsInvisible)
                    break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
