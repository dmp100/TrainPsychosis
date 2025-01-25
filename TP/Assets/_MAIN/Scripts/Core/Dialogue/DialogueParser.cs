using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace DIALOGUE
{
    public class DialogueParser
    {
        // 한글을 포함한 유니코드 문자를 처리하기 위해 패턴 수정
        private const string commandRegexPattern = "[\\p{L}\\p{N}_]*[^\\s]\\(";

        public static DIALOGUE_LINE Parse(string rawLine)
        {
            Debug.Log($"Parsing line - '{rawLine}'");
            (string speaker, string dialogue, string commands) = RipContent(rawLine);
            Debug.Log($"Speaker = '{speaker}'\nDialogue = '{dialogue}' \nCommands =''{commands}");
            return new DIALOGUE_LINE(speaker, dialogue, commands);
        }

        private static (string, string, string) RipContent(string rawLine)
        {
            string speaker = "", dialogue = "", commands = "";
            int dialogueStart = -1;
            int dialogueEnd = -1;
            bool isEscaped = false;

            for (int i = 0; i < rawLine.Length; i++)
            {
                char current = rawLine[i];
                if (current == '\\')
                    isEscaped = !isEscaped;
                else if (current == '"' && !isEscaped)
                {
                    if (dialogueStart == -1)
                        dialogueStart = i;
                    else if (dialogueEnd == -1)
                        dialogueEnd = i;
                }
                else
                    isEscaped = false;
            }

            // Identify Command Pattern
            Regex commandRegex = new Regex(commandRegexPattern);
            Match match = commandRegex.Match(rawLine);
            int commandStart = -1;

            if (match.Success)
            {
                commandStart = match.Index;
                Debug.Log($"Command match found at index: {commandStart}");
                if (dialogueStart == -1 && dialogueEnd == -1)
                {
                    Debug.Log($"No dialogue found, treating as pure command: {rawLine.Trim()}");
                    return ("", "", rawLine.Trim());
                }
            }

            // If we are here then we either have dialogue or multi word argument in a command. Figure out if this is dialogue
            if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
            {
                // we know that we have valid dialogue
                speaker = rawLine.Substring(0, dialogueStart).Trim();
                dialogue = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");
                if (commandStart != -1)
                {
                    commands = rawLine.Substring(commandStart).Trim();
                    Debug.Log($"Found command after dialogue: {commands}");
                }
            }
            else if (commandStart != -1 && dialogueStart > commandStart)
            {
                commands = rawLine;
                Debug.Log($"Found command before dialogue, treating as pure command: {commands}");
            }
            else
            {
                speaker = rawLine;
                Debug.Log($"No commands or dialogue found, treating as pure speaker: {speaker}");
            }

            return (speaker, dialogue, commands);
        }
    }
}












//using System.Collections;
//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using UnityEngine;
//namespace DIALOGUE
//{
//    public class DialogueParser
//    {
//        private const string commandRegexPattern = "\\w*[^\\s]\\(";
//        public static DIALOGUE_LINE Parse(string rawLine)
//        {
//            Debug.Log($"Parsing line - '{rawLine}'");

//            (string speaker, string dialogue, string commands) = RipContent(rawLine);

//            Debug.Log($"Speaker = '{speaker}'\nDialogue = '{dialogue}' \nCommands =''{commands}");

//            return new DIALOGUE_LINE(speaker, dialogue, commands);
//        }
//        private static (string, string, string) RipContent(string rawLine)
//        {
//            string speaker = "", dialogue = "", commands = "";
//            int dialogueStart = -1;
//            int dialogueEnd = -1;
//            bool isEscaped = false;
//            for (int i = 0; i < rawLine.Length; i++)
//            {
//                char current = rawLine[i];
//                if (current == '\\')
//                    isEscaped = !isEscaped;
//                else if (current == '"' && !isEscaped)
//                {
//                    if (dialogueStart == -1)
//                        dialogueStart = i;
//                    else if (dialogueEnd == -1)
//                        dialogueEnd = i;
//                }
//                else
//                    isEscaped = false;
//            }
//            //Debug.Log(rawLine.Substring(dialogueStart + 1, (dialogueEnd - dialogueStart)-1));

//            // Identify Command Pattern
//            Regex commandRegex = new Regex(commandRegexPattern);
//            Match match = commandRegex.Match(rawLine);
//            int commandStart = -1;
//            if (match.Success)
//            {
//                commandStart = match.Index;

//                if (dialogueStart == -1 && dialogueEnd == -1)
//                    return ("", "", rawLine.Trim());

//                return ("", "", rawLine.Trim());
//            }
//            // If we are here then we either have dialogue or multi word argument in a command. Figure out if this is dialogue
//            if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
//            {
//                // we know that we have valid dialogue
//                speaker = rawLine.Substring(0, dialogueStart).Trim();
//                dialogue = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");
//                if (commandStart != -1)
//                    commands = rawLine.Substring(commandStart).Trim();



//            }  
//            else if (commandStart != -1 && dialogueStart > commandStart)
//                commands = rawLine;
//            else
//                speaker = rawLine;
//            return (speaker, dialogue, commands);
//        }
//    }
//}