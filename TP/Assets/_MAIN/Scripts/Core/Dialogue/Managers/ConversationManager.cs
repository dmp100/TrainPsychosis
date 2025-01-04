using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    public class ConversationManager 
    {

        private DialogueSystem dialogueSystem => DialogueSystem.instance;
        private Coroutine process = null;
        public bool isRunning => process != null;
        private TextArchitect architect = null;
        private bool userPrompt = false;

        public ConversationManager(TextArchitect architect) 
        {
            this.architect = architect;
            dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;
        }

        private void OnUserPrompt_Next()
        {
            userPrompt = true;
        }


        public void StartConversation(List<string> conversation)
        {
            StopConversation();

            process = dialogueSystem.StartCoroutine(RunningCoversation(conversation));
        }

        public void StopConversation()
        {
            if (!isRunning)
                return;

            dialogueSystem.StopCoroutine(process);
            process= null;
        }


        IEnumerator RunningCoversation(List<string> conversation)
        {
            for(int i = 0; i< conversation.Count; i++) 
            {
                //Dont show any blank lines or try to run any logic on them
                if (string.IsNullOrWhiteSpace(conversation[i]))
                    continue;

                DIALOGUE_LINE line = DialogueParser.Parse (conversation[i]);

                //Show dialogue
                
                if(line.hasDialogue)
                    yield return Line_Rundialogue(line);
               

                //Run any commands

                if(line.hasCommands)
                    yield return Line_RunCommands(line);

               // yield return new WaitForSeconds(1); // 1ÃÊ µô·¹ÀÌ


            }
        }

        IEnumerator Line_Rundialogue(DIALOGUE_LINE line)
        {
            //show or hide the speaker name if there is one present
            if (line.hasSpeaker)
                dialogueSystem.ShowSpeakerName(line.speaker);
            else
                dialogueSystem.HideSpeakerName();

            //Build dialogue
            yield return BuildDialogue(line.dialogue);

            //Wait for user Input
            yield return waitForUserInput();

        }

        IEnumerator Line_RunCommands(DIALOGUE_LINE line)
        {
            Debug.Log(line.commands);
            yield return null;
        }
        IEnumerator BuildDialogue(string dialogue)
        {
            architect.Build(dialogue);

            while (architect.isBuilding)
            {
                if (userPrompt)
                {
                    if (!architect.hurryup)
                        architect.hurryup = true;
                    else
                        architect.ForceComplete();

                    userPrompt = false;

                }
                yield return null;
            }
        }


        IEnumerator waitForUserInput()
        {
            while(!userPrompt)
                yield return null;

            userPrompt= false;
        }
    }
}