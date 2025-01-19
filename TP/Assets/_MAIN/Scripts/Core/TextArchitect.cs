using System.Collections;
using UnityEngine;
using TMPro;

public class TextArchitect
{
    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_world;
    /// <summary>
    /// The assigned text component for this architect.
    /// </summary>
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    /// <summary>
    /// The text built by this architect.
    /// </summary>
    public string currentText { get { return tmpro.text; } }
    /// <summary>
    /// The current text that this architect is trying to build. This is excluding any pretext that might be assigned for appending text.
    /// </summary>
    public string targetText { get; private set; } = "";
    /// <summary>
    /// The text that should exist prior to an appending build.
    /// </summary>
    public string preText { get; private set; } = "";
    private int preTextLength = 0;
    /// <summary>
    /// The full text that this architect is trying to display, including the pre text that may have existed before appending the new target text..
    /// </summary>
    public string fullTargetText => preText + targetText;

    /// <summary>
    /// Different methods that are available to this architect to render text with.
    /// </summary>
    public enum BuildMethod { instant, typeWriter, fade }
    /// <summary>
    /// How is the text build for this architect? How are the characters revealed?
    /// </summary>
    public BuildMethod buildMethod = BuildMethod.typeWriter;

    /// <summary>
    /// The color that is rendering on this text architect's tmpro component.
    /// </summary>
    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    /// <summary>
    /// How fast text building is determined by the speed
    /// </summary>
    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private float baseSpeed = 1;
    /// <summary>
    /// In addition to the base speed of the architect, the speed multiplier will affect it as well.
    /// </summary>
    private float speedMultiplier = 1;

    /// <summary>
    /// How many characters will be built per frame. When used with the fade technique, this instead just multiplies the speed.
    /// </summary>
    public int charactersPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } set { characterMultiplier = value; } }
    /// <summary>
    /// Multiply the charactersPerFrame By This.
    /// </summary>
    private int characterMultiplier = 1;

    /// <summary>
    /// if the architect is set to rush, it will display text much faster than normal.
    /// </summary>
    public bool hurryUp = false;

    /// <summary>
    /// Create a text architect using this ui text object
    /// </summary>
    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }
    /// <summary>
    /// Create a text architect using this text object
    /// </summary>
    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }

    /// <summary>
    /// Build and display a string using this text.
    /// </summary>
    /// <param name="text"></param>
    public Coroutine Build(string text)
    {
        preText = "";
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    /// <summary>
    /// Append and build a string to what is already being displayed on this text
    /// </summary>
    /// <param name="text"></param>
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
    /// Is this architect building its text at the moment?
    /// </summary>
    public bool isBuilding => buildProcess != null;

    /// <summary>
    /// Stop building the text. This will not finish the text, but stop it where it is immediately.
    /// </summary>
    public void Stop()
    {
        if (!isBuilding)
            return;

        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    /// <summary>
    /// This is what happens when the text has finished completing.
    /// </summary>
    private void OnComplete()
    {
        buildProcess = null;
        hurryUp = false;
    }

    /// <summary>
    /// Stop any active build process immediately and complete the text.
    /// </summary>
    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.typeWriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
            case BuildMethod.fade:
                textColor = new Color(textColor.r, textColor.g, textColor.b, 1);
                break;
        }

        //Stop the build process if it is running.
        Stop();
        OnComplete();
    }

    /// <summary>
    /// Prepare the assigned text component for the build process.
    /// </summary>
    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.instant:
                Prepare_Instant();
                break;
            case BuildMethod.typeWriter:
                Prepare_Typewriter();
                break;
            case BuildMethod.fade:
                Prepare_Fade();
                break;
        }
    }

    private void Prepare_Instant()
    {
        textColor = textColor;//new Color(textColor.r, textColor.g, textColor.b, 1);
        tmpro.text = fullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }

    private void Prepare_Typewriter()
    {
        //Recover color in case this architect used fading earlier.
        textColor = textColor;// new Color(textColor.r, textColor.g, textColor.b, 1);
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
        //Get the pretext length if there is any so we know what should start visible.
        tmpro.text = preText;
        if (preText != "")
        {
            tmpro.ForceMeshUpdate();
            preTextLength = tmpro.textInfo.characterCount;
        }
        else
            preTextLength = 0;

        //Now add the target text to complete the string.
        tmpro.text += targetText;

        tmpro.maxVisibleCharacters = int.MaxValue;
        tmpro.ForceMeshUpdate();
        TMP_TextInfo textInfo = tmpro.textInfo;

        Color colorVisable = new Color(textColor.r, textColor.g, textColor.b, 1);
        Color colorHidden = new Color(textColor.r, textColor.g, textColor.b, 0);

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;

        // Loop through all characters and set the right starting color.
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            // Get the vertex colors of the current character
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            //Invisible characters have their index default to the first character in the string.
            //In order to avoid overwriting the first character with a secondary color, skip any invisible characters
            if (!charInfo.isVisible)
                continue;

            //Debug.Log($"Examine '{charInfo.character}' [{i}/{preTextLength}] = {(i<preTextLength ? "red" : "black")}");

            // Set the color of the current character
            if (i < preTextLength)
            {
                //Debug.Log($"set pre '{charInfo.character}' to red");
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorVisable;
            }
            else
            {
                //Debug.Log($"set new '{charInfo.character}' to black");  
                for (int v = 0; v < 4; v++)
                    vertexColors[charInfo.vertexIndex + v] = colorHidden;
            }
        }

        // Update the text to reflect the new colors
        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    IEnumerator Build_Typewriter()
    {
        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += hurryUp ? charactersPerCycle * 5 : charactersPerCycle;

            yield return new WaitForSeconds(0.015f / speed);
        }
    }

    IEnumerator Build_Fade()
    {
        //Create a min and max char limit to control how many characters are allowed to render. This allows us to skip pretext and also do a sweeping fade.
        int minChar = preTextLength;
        int maxChar = preTextLength + 1;
        //Alpha threshold is how visible a character should be before increasing the max char limit
        byte alphaThreshold = 15;

        TMP_TextInfo textInfo = tmpro.textInfo;

        // Get the vertex colors of the mesh used by this text element (character or sprite).
        Color32[] newVertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];

        while (true)
        {
            //Multiplying the speed by 1.6f makes the fade last as long as the typewriter.
            float fadeSpeed = ((hurryUp ? charactersPerCycle * 5f : charactersPerCycle) * speed) * 4f;

            for (int i = minChar; i < maxChar; i++)
            {
                //Invisible characters like spaces have a vertex index of 0 which means if we change the color on that - it will try to set the color of the first character in the text.
                //Running this in a loop causes the first character to consistently flicker until the fade is complete.
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                alphas[i] = Mathf.MoveTowards(alphas[i], 255, fadeSpeed);

                for (int v = 0; v < 4; v++)
                    newVertexColors[vertexIndex + v].a = (byte)alphas[i];

                if (alphas[i] >= 255)
                    minChar++;
            }

            // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            //If the last character is invisible then try to increase the max character limit on the sweeping fade.
            //Do this if the current character has passed the alpha threshold.
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
            case BuildMethod.typeWriter:
                yield return Build_Typewriter();
                break;
            case BuildMethod.fade:
                yield return Build_Fade();
                break;
        }

        OnComplete();
    }

    /// <summary>
    /// Immediately set the text of this architect to this text, bypassing any build requirement without changing the build method
    /// </summary>
    public void SetText(string text)
    {
        //Stop any active build processes.
        Stop();

        tmpro.text = text;
        TMP_TextInfo info = tmpro.textInfo;

        tmpro.ForceMeshUpdate();

        //Set the visible characters and the color to account for whatever buld method may be used or may have been used.
        tmpro.maxVisibleCharacters = info.characterCount;
        textColor = new Color(textColor.r, textColor.g, textColor.b, 1);
    }
}