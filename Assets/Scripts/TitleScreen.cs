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
    [SerializeField] List<Button> buttonSettings;

    private void Start()
    {
        for (int i = 0; i < buttonSettings.Count; i++)
        {
            Setting enumValue = (Setting)i;
            buttonSettings[i].onClick.AddListener(() => LoadWithSetting(enumValue));
        }

        leftArrow.onClick.AddListener(Decrement);
        rightArrow.onClick.AddListener(Increment);

        DisplayLevel();
    }

    void Increment()
    {
        if (levelToLoad == listOfLevels.Count - 1)
            levelToLoad = 0;
        else
            levelToLoad++;
        DisplayLevel();
    }


    void Decrement()
    {
        if (levelToLoad <= 0)
            levelToLoad = listOfLevels.Count - 1;
        else
            levelToLoad--;
        DisplayLevel();
    }

    void LoadWithSetting(Setting setting)
    {
        LevelSettings.instance.setting = setting;
        SceneManager.LoadScene(levelToLoad+1);
    }

    void DisplayLevel()
    {
        levelText.text = listOfLevels[levelToLoad].name;
        levelImage.sprite = listOfLevels[levelToLoad].sprite;
    }
}
