using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyBox;
using System;
using System.Reflection;

public class Translator : MonoBehaviour
{

#region Setup

    public static Translator inst;
    Dictionary<string, Dictionary<string, string>> keyTranslate = new();
    [Scene][SerializeField] string toLoad;

    void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(this.gameObject);
            Application.targetFrameRate = 60;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Language"))
            PlayerPrefs.SetString("Language", "English");

        TxtLanguages();
        CsvLanguages(ReadFile("Csv Languages"));
        SceneManager.LoadScene(toLoad);
    }

    #endregion

#region Reading Files

    public static string[][] ReadFile(string range)
    {
        TextAsset data = Resources.Load($"{range}") as TextAsset;

        string editData = data.text;
        editData = editData.Replace("],", "").Replace("{", "").Replace("}", "");

        string[] numLines = editData.Split("[");
        string[][] list = new string[numLines.Length][];

        for (int i = 0; i < numLines.Length; i++)
            list[i] = numLines[i].Split("\",");
        return list;
    }

    void TxtLanguages()
    {
        TextAsset[] languageFiles = Resources.LoadAll<TextAsset>("Txt Languages");
        foreach (TextAsset language in languageFiles)
        {
            (bool success, string converted) = ConvertTxtName(language);
            if (success)
            {
                Dictionary<string, string> newDictionary = new();
                keyTranslate.Add(converted, newDictionary);
                string[] lines = language.text.Split('\n');

                foreach (string line in lines)
                {
                    if (line != "")
                    {
                        string[] parts = line.Split('=');
                        string key = FixLine(parts[0]).Replace(" ", "_");
                        if (newDictionary.ContainsKey(key))
                            Debug.Log($"ignore duplicate: {key}");
                        else
                            newDictionary[FixLine(key)] = FixLine(parts[1]);
                    }
                }
            }
        }

        (bool, string) ConvertTxtName(TextAsset asset)
        {
            //pattern: "0. English"
            string pattern = @"^\d+\.\s*(.+)$";
            Match match = Regex.Match(asset.name, pattern);
            if (match.Success)
                return (true, match.Groups[1].Value);
            else
                return (false, "");
        }
    }

    public static string FixLine(string line)
    {
        return line.Replace("\"", "").Replace("\\", "").Replace("]", "").Replace("|", "\n").Trim();
    }

    void CsvLanguages(string[][] data)
    {
        for (int i = 1; i < data[1].Length; i++)
        {
            data[1][i] = data[1][i].Replace("\"", "").Trim();
            Dictionary<string, string> newDictionary = new();
            keyTranslate.Add(data[1][i], newDictionary);
        }

        for (int i = 2; i < data.Length; i++)
        {
            for (int j = 0; j < data[i].Length; j++)
            {
                data[i][j] = FixLine(data[i][j]);
                if (j > 0)
                {
                    string language = data[1][j];
                    string key = data[i][0].Replace(" ", "_");
                    if (keyTranslate[language].ContainsKey(key))
                        Debug.Log($"ignore duplicate: {key}");
                    else
                        keyTranslate[language][key] = data[i][j];
                }
            }
        }
    }
 
    #endregion

#region Helpers

    public bool TranslationExists(string key)
    {
        return keyTranslate["English"].ContainsKey(key);
    }

    public string Translate(string key, List<(string, string)> toReplace = null)
    {
        string answer;
        try
        {
            answer = keyTranslate[PlayerPrefs.GetString("Language")][key];
        }
        catch
        {
            try
            {
                answer = keyTranslate[("English")][key];
                //Debug.Log($"{key} failed to translate in {PlayerPrefs.GetString("Language")}");
            }
            catch
            {
                //Debug.Log($"{key} failed to translate at all");
                return key;
            }
        }

        if (toReplace != null)
        {
            foreach ((string one, string two) in toReplace)
                answer = answer.Replace($"${one}$", two);
        }
        return answer;
    }

    public Dictionary<string, Dictionary<string, string>> GetTranslations()
    {
        return keyTranslate;
    }

    public void ChangeLanguage(string newLanguage)
    {
        if (!PlayerPrefs.GetString("Language").Equals(newLanguage))
        {
            PlayerPrefs.SetString("Language", newLanguage);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    #endregion

}
