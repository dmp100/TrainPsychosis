using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class TESTPARCING : MonoBehaviour
    {

        [SerializeField] private TextAsset file;

        void Start()
        {
            //string line = "Speaker \"Dialogue \\\"goes In\\\" here !\" Command(arguments here)";
            //DialogueParser.Parse(line);


            SendFileToParse();



        }

        void SendFileToParse()
        {
            List<string> lines = FileManagers.ReadTextAsset("dd");

            foreach (string line in lines) 
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                DIALOGUE_LINE dl = DialogueParser.Parse(line);
            }

        }
    }
}