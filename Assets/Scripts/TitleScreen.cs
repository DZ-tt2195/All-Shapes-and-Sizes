using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct LevelInfo
{
    public string name;
    public Sprite sprite;
}

[System.Serializable]
public struct ButtonInfo
{
    public Button button;
    public Image image;
}


public class TitleScreen : MonoBehaviour
{
    int levelToLoad = 0;
    [SerializeField] List<LevelInfo> listOfLevels = new();

    [SerializeField] Image levelImage;
    [SerializeField] TMP_Text levelText;
    [SerializeField] Button leftArrow;
    [SerializeField] Button rightArrow;

    [SerializeField] TMP_Text endlessHighScore;
    public enum Setting { MergeCrown, ReachScore, Endless };
    [SerializeField] List<ButtonInfo> buttonSettings;

    private void Start()
    {
        levelToLoad = LevelSettings.instance.lastLevel;
        for (int i = 0; i < buttonSettings.Count; i++)
        {
            Setting enumValue = (Setting)i;
            buttonSettings[i].button.onClick.AddListener(() => LoadWithSetting(enumValue));
        }

        leftArrow.onClick.AddListener(Decrement);
        rightArrow.onClick.AddListener(Increment);
        levelToLoad = LevelSettings.instance.lastLevel;
        DisplayLevel();
    }

    void Increment()
    {
        levelToLoad = (levelToLoad == listOfLevels.Count - 1) ? 0 : levelToLoad + 1;
        DisplayLevel();
    }

    void Decrement()
    {
        levelToLoad = (levelToLoad <= 0 ) ? listOfLevels.Count - 1 : levelToLoad - 1;
        DisplayLevel();
    }

    void LoadWithSetting(Setting setting)
    {
        LevelSettings.instance.setting = setting;
        LevelSettings.instance.lastLevel = levelToLoad;
        SceneManager.LoadScene(listOfLevels[levelToLoad].name);
    }

    void DisplayLevel()
    {
        levelText.text = listOfLevels[levelToLoad].name;
        levelImage.sprite = listOfLevels[levelToLoad].sprite;

        buttonSettings[0].image.color = (PlayerPrefs.GetInt($"{listOfLevels[levelToLoad].name} - Merge") == 1) ? Color.yellow : Color.white;
        buttonSettings[1].image.color = (PlayerPrefs.GetInt($"{listOfLevels[levelToLoad].name} - Score") == 1) ? Color.yellow : Color.white;
        endlessHighScore.text = $"High Score: {PlayerPrefs.GetInt($"{listOfLevels[levelToLoad].name} - Endless")}";
    }
}