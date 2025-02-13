using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    public class DIALOGUE_LINE
    {
        public DL_SPEAKER_DATA speakerData;
        public DL_DIALOGUE_DATA dialogueData;
        public DL_COMMAND_DATA commandData;

        public bool hasSpeaker => speakerData != null;// speaker != string.Empty;
        public bool hasDialogue => dialogueData != null;
        public bool hasCommands => commandData != null;

        public DIALOGUE_LINE(string speaker, string dialogue, string commands)
        {
            this.speakerData = (string.IsNullOrWhiteSpace(speaker) ? null : new DL_SPEAKER_DATA(speaker));
            this.dialogueData = (string.IsNullOrWhiteSpace(dialogue) ? null : new DL_DIALOGUE_DATA(dialogue));
            this.commandData = (string.IsNullOrWhiteSpace(commands) ? null : new DL_COMMAND_DATA(commands));
        }
    }

    //public class DIALOGUE_LINE
    //{
    //    public string speaker;
    //    public string dialogue;
    //    public string commands;



    //    public bool hasSpeaker => speaker != string.Empty;

    //    public bool hasDialogue => dialogue != string.Empty;

    //    public bool hasCommands => commands != string.Empty;

    //    public DIALOGUE_LINE(string speaker, string dialogue, string commands)
    //    {
    //        this.speaker = speaker;
    //        this.dialogue = dialogue;
    //        this.commands = commands;
    //    }
    //}
}