using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;  // Encoding을 사용하기 위해 추가
using UnityEngine;

public class FileManagers
{
    public static List<string> ReadTextFile(string filePath, bool includeBlankLines = true)
    {
        // 파일 경로를 먼저 처리합니다
        if (filePath.StartsWith('/'))
        {
            filePath = FilePaths.root + filePath;
        }

        List<string> lines = new List<string>();
        try
        {
            // BOM을 감지하고 UTF-8 인코딩을 명시적으로 지정합니다
            using (StreamReader sr = new StreamReader(filePath, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
            {
                // 현재 사용 중인 인코딩을 로그로 출력하여 확인합니다
                Debug.Log($"Current file encoding: {sr.CurrentEncoding.EncodingName}");

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    // 읽어들인 각 라인의 바이트 정보를 확인합니다
                    byte[] bytes = Encoding.UTF8.GetBytes(line);
                    Debug.Log($"Line bytes: {BitConverter.ToString(bytes)}");

                    if (includeBlankLines || !string.IsNullOrWhiteSpace(line))
                    {
                        lines.Add(line);
                    }
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            Debug.LogError($"File not found: '{ex.FileName}'");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error reading file: {ex.Message}");
        }

        return lines;
    }

    public static List<string> ReadTextAsset(TextAsset asset, bool includeBlankLines = true)
    {
        List<string> lines = new List<string>();

        try
        {
            // TextAsset의 텍스트를 바이트 배열로 변환한 후 다시 문자열로 변환합니다
            byte[] bytes = Encoding.UTF8.GetBytes(asset.text);
            string text = Encoding.UTF8.GetString(bytes);

            using (StringReader sr = new StringReader(text))
            {
                while (sr.Peek() > -1)
                {
                    string line = sr.ReadLine();

                    // 각 라인의 인코딩 상태를 확인합니다
                    byte[] lineBytes = Encoding.UTF8.GetBytes(line);
                    Debug.Log($"Asset line bytes: {BitConverter.ToString(lineBytes)}");

                    if (includeBlankLines || !string.IsNullOrWhiteSpace(line))
                    {
                        lines.Add(line);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error reading TextAsset: {ex.Message}");
        }

        return lines;
    }
}