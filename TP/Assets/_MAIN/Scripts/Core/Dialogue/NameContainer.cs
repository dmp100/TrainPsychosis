using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
///  The box that holds the name text on screen. Part of the dialogue container
/// </summary>

namespace DIALOGUE
{

    [System.Serializable]

    public class NameContainer
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TextMeshProUGUI nameText;

        public void Show(string nameToshow = "")
        {
            root.SetActive(true);

            if (nameToshow != string.Empty)
                nameText.text = nameToshow;

        }

        public void Hide()
        {
            root.SetActive(false);
        }
    }
}