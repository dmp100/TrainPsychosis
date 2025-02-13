using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE
{
    public class DialogueParser
    {
        // 명령어 패턴을 위한 정규식
        // 한글, 영문, 숫자, 밑줄(_)을 포함하며 공백이 아닌 문자로 끝나고 괄호로 끝나는 패턴
        private const string commandRegexPattern = "[\\p{L}\\p{N}_]*[^\\s]\\(";

        /// <summary>
        /// 원시 문자열을 파싱하여 DIALOGUE_LINE 객체로 변환
        /// </summary>
        /// <param name="rawLine">파싱할 원시 문자열</param>
        /// <returns>파싱된 DIALOGUE_LINE 객체</returns>
        public static DIALOGUE_LINE Parse(string rawLine)
        {
            try
            {
                // 입력 문자열의 인코딩을 확인하고 필요한 경우 변환합니다
                byte[] bytes = Encoding.UTF8.GetBytes(rawLine);
                string encodedLine = Encoding.UTF8.GetString(bytes);

                Debug.Log($"Parsing encoded line: {encodedLine}");
                Debug.Log($"Line bytes: {BitConverter.ToString(bytes)}");

                (string speaker, string dialogue, string commands) = RipContent(encodedLine);

                // 파싱된 결과의 인코딩도 확인합니다
                if (!string.IsNullOrEmpty(dialogue))
                {
                    byte[] dialogueBytes = Encoding.UTF8.GetBytes(dialogue);
                    Debug.Log($"Dialogue bytes: {BitConverter.ToString(dialogueBytes)}");
                }

                return new DIALOGUE_LINE(speaker, dialogue, commands);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing line: {ex.Message}");
                return new DIALOGUE_LINE("", "", "");
            }
        }


        /// <summary>
        /// 원시 문자열에서 화자, 대화, 명령어를 추출
        /// </summary>
        /// <param name="rawLine">처리할 원시 문자열</param>
        /// <returns>(화자, 대화, 명령어) 튜플</returns>
        private static (string, string, string) RipContent(string rawLine)
        {
            // 변수 선언은 한 번만
            string speaker = "", dialogue = "", commands = "";
            int dialogueStart = -1;
            int dialogueEnd = -1;
            bool isEscaped = false;
            // 큰따옴표 위치 찾기 (이스케이프 처리 포함)
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

            // 명령어 패턴 식별
            Regex commandRegex = new Regex(commandRegexPattern);
            Match match = commandRegex.Match(rawLine);
            int commandStart = -1;

            if (match.Success)
            {
                commandStart = match.Index;
                Debug.Log($"Command match found at index: {commandStart}");

                // 대화가 없는 순수 명령어인 경우
                if (dialogueStart == -1 && dialogueEnd == -1)
                {
                    Debug.Log($"No dialogue found, treating as pure command: {rawLine.Trim()}");
                    return ("", "", rawLine.Trim());
                }
            }

            // 대화 또는 명령어 인자 처리
            if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
            {
                // 유효한 대화가 있는 경우
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
                // 대화 이전에 명령어가 있는 경우
                commands = rawLine;
                Debug.Log($"Found command before dialogue, treating as pure command: {commands}");
            }
            else
            {
                // 명령어나 대화가 없는 경우
                speaker = rawLine;
                Debug.Log($"No commands or dialogue found, treating as pure speaker: {speaker}");
            }

            return (speaker, dialogue, commands);
        }
    }
}