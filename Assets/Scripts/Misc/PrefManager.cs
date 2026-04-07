using System.ComponentModel;
using UnityEngine;

public static class PrefManager
{

    public static Setting GetSetting() => (Setting)PlayerPrefs.GetInt("Setting");
    public static void SetSetting(Setting setting) => PlayerPrefs.SetInt("Setting", (int)setting);

    public static int GetScore(Setting setting) => PlayerPrefs.GetInt($"{setting}");
    public static void SetScore(Setting setting, int value) => PlayerPrefs.SetInt($"{setting}", value);

}

