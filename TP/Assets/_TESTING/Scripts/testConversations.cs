using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testConversations : MonoBehaviour
{
    

    void Start()
    {

        StartConversation();

    }

    void StartConversation()
    {
        List<string> lines = FileManagers.ReadTextAsset("dd");
        Debug.Log($"읽어들인 라인 수: {lines.Count}");

        foreach (string line in lines)
        {
            Debug.Log($"읽은 라인: {line}");
        }


        DialogueSystem.instance.Say(lines);

    }
}
