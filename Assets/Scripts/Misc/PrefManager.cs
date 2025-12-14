using System.ComponentModel;
using UnityEngine;

public static class PrefManager
{
    public static ToTranslate GetLevel() => (ToTranslate)PlayerPrefs.GetInt("Level");
    public static void SetLevel(ToTranslate newLevel) => PlayerPrefs.SetInt("Level", (int)newLevel);

    public static Setting GetSetting() => (Setting)PlayerPrefs.GetInt("Setting");
    public static void SetSetting(Setting setting) => PlayerPrefs.SetInt("Setting", (int)setting);

    public static int GetScore(ToTranslate levelName, Setting setting) => PlayerPrefs.GetInt($"{levelName} - {setting}");
    public static void SetScore(ToTranslate levelName, Setting setting, int value) => PlayerPrefs.SetInt($"{levelName} - {setting}", value);

}

