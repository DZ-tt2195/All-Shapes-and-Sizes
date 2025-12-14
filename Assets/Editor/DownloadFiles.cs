using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;
using System.Text.RegularExpressions;

public static class FileManager
{
    private static string sheetURL = "1Nx9-tZf2aTOOsb4ZYs-LTxHY13DNfRYmGSbC87azuqE";
    private static string apiKey = "AIzaSyCl_GqHd1-WROqf7i2YddE3zH6vSv3sNTA";
    private static string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets/";

    [MenuItem("Tools/Download from spreadsheet")]
    public static void DownloadFiles()
    {
        Debug.Log($"starting downloads");
        EditorCoroutineUtility.StartCoroutineOwnerless(Download("Csv Languages"));
    }
    static IEnumerator Download(string range)
    {
        string url = $"{baseUrl}{sheetURL}/values/{range}?key={apiKey}";
        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Download failed: {www.error}");
        }
        else
        {
            string filePath = $"Assets/Resources/{range}.txt";
            File.WriteAllText($"{filePath}", www.downloadHandler.text);

            string[] allLines = File.ReadAllLines($"{filePath}");
            List<string> modifiedLines = allLines.ToList();
            modifiedLines.RemoveRange(1, 3);
            File.WriteAllLines($"{filePath}", modifiedLines.ToArray());
            Debug.Log($"downloaded {range}");
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Tools/Create base txt")]
    public static void CreateBaseTxtFile()
    {
        string baseText = "";
        string[][] csvFile = Translator.ReadFile("Csv Languages");
        List<string> noConvert = new();
        List<(string, List<string>)> needConvert = new();

        for (int i = 2; i < csvFile.Length; i++)
        {
            string key = Translator.FixLine(csvFile[i][0]).Replace(" ", "_");
            string value = Translator.FixLine(csvFile[i][1]).Replace("u003e", ">");
            baseText += $"{key}={value}\n";

            Regex regex = new(@"\$(.*?)\$");
            List<string> allMatches = new();
            foreach (Match m in regex.Matches(value).Cast<Match>())
            {
                string match = m.Groups[1].Value;
                allMatches.Add(match);
            }

            if (allMatches.Count == 0)
                noConvert.Add(key);
            else
                needConvert.Add((key, allMatches));
        }

        File.WriteAllText($"Assets/Resources/BaseTxtFile.txt", baseText);
        Debug.Log($"converted English csv to txt file");

        using (StreamWriter writer = new StreamWriter("Assets/Scripts/Translations/AutoTranslate.cs"))
        {
            writer.WriteLine("using System.Collections.Generic;\npublic static class AutoTranslate\n{\n");

            for (int i = 0; i < needConvert.Count; i++)
            {
                (string key, List<string> replace) = needConvert[i];
                string nextCode = $"public static string {key} (";
                for (int j = 0; j < replace.Count; j++)
                {
                    nextCode += $"string {replace[j]}";
                    if (j < replace.Count - 1)
                        nextCode += ",";
                }
                nextCode += ")  { return(";
                nextCode += $"Translator.inst.Translate(\"{key}\", new()";
                nextCode += "{";

                for (int j = 0; j < replace.Count; j++)
                {
                    nextCode += $"(\"{replace[j]}\", {replace[j]})";
                    if (j < replace.Count - 1)
                        nextCode += ",";
                }
                nextCode += "})); }\n";
                writer.WriteLine(nextCode);
            }
            
            writer.WriteLine("public static string DoEnum(ToTranslate thing) {return(Translator.inst.Translate(thing.ToString()));}");
            
            writer.WriteLine("}");

            writer.WriteLine("public enum ToTranslate {");
            string nextEnum = "";
            for (int i = 0; i < noConvert.Count; i++)
            {
                nextEnum += $"{noConvert[i]}";
                if (i < noConvert.Count - 1)
                    nextEnum += ",";
            }
            nextEnum += "}";
            writer.WriteLine(nextEnum);
        }
        Debug.Log($"{noConvert.Count} enum lines, {needConvert.Count} converted lines");

        /*
        string filePath = Path.Combine(Application.persistentDataPath, "BaseTxtFile.txt");
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (string input in listOfKeys)
                writer.WriteLine($"{input}=");
        }*/

        AssetDatabase.Refresh();
    }
}
