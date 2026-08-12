using UnityEngine;

public static class PrefManager
{
    public static GameMode GetMode() => (GameMode)PlayerPrefs.GetInt("Mode");
    public static void SetMode(GameMode setting) => PlayerPrefs.SetInt("Mode", (int)setting);

    public static int GetScore(GameMode mode) 
    {
        if (!PlayerPrefs.HasKey(mode.ToString()))
            return -1;
        else
            return PlayerPrefs.GetInt($"{mode}");
    }
    public static void SetScore(GameMode mode, int value) => PlayerPrefs.SetInt($"{mode}", value);

    public static int GetShape(int number) => PlayerPrefs.HasKey($"Shape {number}") ? PlayerPrefs.GetInt($"Shape {number}") : -1;
    public static void SetShape(int number, int value) => PlayerPrefs.SetInt($"Shape {number}", value);
}

