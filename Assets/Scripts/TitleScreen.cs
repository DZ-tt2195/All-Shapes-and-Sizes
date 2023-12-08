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
    public GameObject achievement;
}

public class TitleScreen : MonoBehaviour
{
    int levelToLoad = 0;
    [SerializeField] List<LevelInfo> listOfLevels = new();

    [SerializeField] TMP_Dropdown fpsSetting;
    [SerializeField] Image levelImage;
    [SerializeField] TMP_Text levelText;
    [SerializeField] Button clearData;
    [SerializeField] Button leftArrow;
    [SerializeField] Button rightArrow;

    [SerializeField] TMP_Text endlessHighScore;
    public enum Setting { MergeCrown, ReachScore, Endless };
    [SerializeField] List<ButtonInfo> buttonSettings;

    private void Start()
    {
        levelToLoad = LevelSettings.instance.lastLevel;
        fpsSetting.value = (Application.targetFrameRate == 60) ? 0 : 1;

        for (int i = 0; i < buttonSettings.Count; i++)
        {
            Setting enumValue = (Setting)i;
            buttonSettings[i].button.onClick.AddListener(() => LoadWithSetting(enumValue));
        }

        leftArrow.onClick.AddListener(Decrement);
        rightArrow.onClick.AddListener(Increment);
        clearData.onClick.AddListener(ResetData);
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
        Application.targetFrameRate = fpsSetting.value == 0 ? 60 : 30;
        LevelSettings.instance.setting = setting;
        LevelSettings.instance.lastLevel = levelToLoad;
        SceneManager.LoadScene(listOfLevels[levelToLoad].name);
    }

    void DisplayLevel()
    {
        levelText.text = listOfLevels[levelToLoad].name;
        levelImage.sprite = listOfLevels[levelToLoad].sprite;

        buttonSettings[0].image.color = (PlayerPrefs.GetInt($"{listOfLevels[levelToLoad].name} - Merge") >= 1) ? Color.yellow : Color.white;
        buttonSettings[0].achievement.SetActive(PlayerPrefs.GetInt($"{listOfLevels[levelToLoad].name} - Merge") >= 50);

        buttonSettings[1].image.color = (PlayerPrefs.GetInt($"{listOfLevels[levelToLoad].name} - Score") >= 1) ? Color.yellow : Color.white;
        buttonSettings[1].achievement.SetActive(PlayerPrefs.GetInt($"{listOfLevels[levelToLoad].name} - Score") >= 50);

        endlessHighScore.text = $"High Score: {PlayerPrefs.GetInt($"{listOfLevels[levelToLoad].name} - Endless")}";
    }

    void ResetData()
    {
        for (int i = 0; i < listOfLevels.Count; i++)
        {
            PlayerPrefs.SetInt($"{listOfLevels[i].name} - Merge", 0);
            PlayerPrefs.SetInt($"{listOfLevels[i].name} - Score", 0);
            PlayerPrefs.SetInt($"{listOfLevels[i].name} - Endless", 0);
        }

        DisplayLevel();
    }
}
