using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// TextMeshPro(TMP) 컴포넌트에 텍스트를 동적으로 출력하는 기능을 구현하는 클래스입니다.
/// 타자 효과, 페이드 효과 등 다양한 텍스트 출력 방식을 지원합니다.
/// </summary>
public class TextArchitect
{
    // UI용 TextMeshProUGUI 컴포넌트 참조
    private TextMeshProUGUI tmpro_ui;
    // 월드 공간용 TextMeshPro 컴포넌트 참조
    private TextMeshPro tmpro_world;
    /// <summary>
    /// 현재 할당된 텍스트 컴포넌트입니다.
    /// </summary>
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    /// <summary>
    /// 현재 표시되고 있는 텍스트입니다.
    /// </summary>
    public string currentText => tmpro.text;
    /// <summary>
    /// 이 아키텍트가 구성하려는 현재 텍스트입니다. 텍스트 추가를 위해 할당된 이전 텍스트는 제외됩니다.
    /// </summary>
    public string targetText { get; private set; } = "";
    /// <summary>
    /// 텍스트 추가 전에 존재해야 하는 이전 텍스트입니다.
    /// </summary>
    public string preText { get; private set; } = "";
    private int preTextLength = 0;
    /// <summary>
    /// 이전 텍스트와 목표 텍스트를 포함한 전체 목표 텍스트입니다.
    /// </summary>
    public string fullTargetText => preText + targetText;

    /// <summary>
    /// 텍스트 출력에 사용 가능한 다양한 방식입니다.
    /// </summary>
    public enum BuildMethod { instant, typewriter, fade }
    /// <summary>
    /// 텍스트가 어떻게 출력되고 문자가 어떻게 표시될지 결정하는 방식입니다.
    /// </summary>
    public BuildMethod buildMethod = BuildMethod.typewriter;

    /// <summary>
    /// 텍스트의 색상을 설정하거나 가져옵니다.
    /// </summary>
    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    /// <summary>
    /// 텍스트 출력 속도는 기본 속도와 속도 배율에 의해 결정됩니다.
    /// </summary>
    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private const float baseSpeed = 1;
    /// <summary>
    /// 기본 속도에 추가로 적용되는 속도 배율입니다.
    /// </summary>
    private float speedMultiplier = 1;

    /// <summary>
    /// 프레임당 출력될 문자 수입니다. 페이드 효과에서는 속도 승수로 사용됩니다.
    /// </summary>
    public int charactersPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    private int characterMultiplier = 1;

    /// <summary>
    /// 활성화되면 텍스트가 평소보다 훨씬 빠르게 표시됩니다.
    /// </summary>
    public bool hurryup = false;

    /// <summary>
    /// UI 텍스트 객체를 사용하여 텍스트 아키텍트를 생성합니다.
    /// </summary>
    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }

    /// <summary>
    /// 월드 텍스트 객체를 사용하여 텍스트 아키텍트를 생성합니다.
    /// </summary>
    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }

    /// <summary>
    /// 주어진 텍스트를 구성하여 표시합니다.
    /// </summary>
    public Coroutine Build(string text)
    {
        // 폰트 설정이 유지되는지 확인
        preText = "";
        targetText = text;
        Stop();
        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    /// <summary>
    /// 현재 표시된 텍스트에 새로운 텍스트를 추가하여 구성합니다.
    /// 현재 표시된 텍스트에 새로운 텍스트를 추가하여 구성합니다.
    /// </summary>
    public Coroutine Append(string text)
    {
        preText = tmpro.text;
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    private Coroutine buildProcess = null;
    /// <summary>
    /// 현재 텍스트를 구성 중인지 여부를 반환합니다.
    /// </summary>
    public bool isBuilding => buildProcess != null;

    /// <summary>
    /// 텍스트 구성을 중지합니다. 텍스트를 완성하지 않고 즉시 현재 상태에서 중지합니다.
    /// </summary>
    public void Stop()
    {
        if (!isBuilding)
            return;

        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    /// <summary>
    /// 텍스트 구성이 완료되었을 때 호출됩니다.
    /// </summary>
    private void OnComplete()
    {
        buildProcess = null;
        hurryup = false;
    }

    /// <summary>
    /// 진행 중인 구성 과정을 즉시 중지하고 텍스트를 완성합니다.
    /// </summary>
    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
            case BuildMethod.fade:
                textColor = new Color(textColor.r, textColor.g, textColor.b, 1);
                break;
        }

        Stop();
        OnComplete();
    }

    /// <summary>
    /// 텍스트 구성 과정을 준비합니다.
    /// </summary>
    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.instant:
                Prepare_Instant();
                break;
            case BuildMethod.typewriter:
                Prepare_Typewriter();
                break;
            case BuildMethod.fade:
                Prepare_Fade();
                break;
        }
    }

    private void Prepare_Instant()
    {
        textColor = textColor;
        tmpro.text = fullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }

    private void Prepare_Typewriter()
    {
        textColor = textColor;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preText;

        if (preText != "")
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        tmpro.text += targetText;
        tmpro.ForceMeshUpdate();
    }

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

        Color colorVisible = new Color(textColor.r, textColor.g, textColor.b, 1);
        Color colorHidden = new Color(textColor.r, textColor.g, textColor.b, 0);

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            if (i < preTextLength)
            {
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorVisible;
            }
            else
            {
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorHidden;
            }
        }

        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private IEnumerator Build_Typewriter()
    {
        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += hurryup ? charactersPerCycle * 5 : charactersPerCycle;
            yield return new WaitForSeconds(0.015f / speed);
        }
    }

    private IEnumerator Build_Fade()
    {
        int minChar = preTextLength;
        int maxChar = preTextLength + 1;
        byte alphaThreshold = 15;

        TMP_TextInfo textInfo = tmpro.textInfo;
        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];

        while (true)
        {
            float fadeSpeed = ((hurryup ? charactersPerCycle * 5f : charactersPerCycle) * speed) * 4f;

            for (int i = minChar; i < maxChar; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                alphas[i] = Mathf.MoveTowards(alphas[i], 255, fadeSpeed);

                for (int v = 0; v < 4; v++)
                    vertexColors[vertexIndex + v].a = (byte)alphas[i];

                if (alphas[i] >= 255)
                    minChar++;
            }

            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            bool lastCharacterIsInvisible = !textInfo.characterInfo[maxChar - 1].isVisible;
            if (alphas[maxChar - 1] >= alphaThreshold || lastCharacterIsInvisible)
            {
                if (maxChar < textInfo.characterCount)
                    maxChar++;
                else if (alphas[maxChar - 1] >= 255 || lastCharacterIsInvisible)
                    break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator Building()
    {
        Prepare();

        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                yield return Build_Typewriter();
                break;
            case BuildMethod.fade:
                yield return Build_Fade();
                break;
        }

        OnComplete();
    }

    /// <summary>
    /// 구성 방식을 변경하지 않고 텍스트를 즉시 설정합니다.
    /// </summary>
    public void SetText(string text)
    {
        Stop();

        tmpro.text = text;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        textColor = new Color(textColor.r, textColor.g, textColor.b, 1);
    }
}