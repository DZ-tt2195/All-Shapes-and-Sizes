using System.ComponentModel;
using UnityEngine;

public static class PrefManager
{
    public static string GetLevel() => PlayerPrefs.GetString("Level");
    public static void SetLevel(string newLevel) => PlayerPrefs.SetString("Level", newLevel);

    public static Setting GetSetting() => (Setting)PlayerPrefs.GetInt("Setting");
    public static void SetSetting(Setting setting) => PlayerPrefs.SetInt("Setting", (int)setting);

    public static int GetScore(string levelName, Setting setting) => PlayerPrefs.GetInt($"{levelName} - {setting}");
    public static void SetScore(string levelName, Setting setting, int value) => PlayerPrefs.SetInt($"{levelName} - {setting}", value);

}

