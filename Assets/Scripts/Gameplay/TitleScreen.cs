using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using UnityEngine.SceneManagement;

public enum Setting { MergeCrown, DropShape, DropEndless, MergeEndless };

[System.Serializable]
public struct LevelInfo
{
    public ToTranslate levelName;
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

    [Foldout("Sound effects", true)]
        [SerializeField] AudioClip menuSound;

    [Foldout("Levels", true)]
        [SerializeField] List<LevelInfo> listOfLevels = new();
        [SerializeField] List<ButtonInfo> buttonSettings;

    [Foldout("Buttons", true)]
        [SerializeField] Button sfxButton;
        [SerializeField] Button clearData;
        [SerializeField] Button leftArrow;
        [SerializeField] Button rightArrow;

    [Foldout("Text and images", true)]
        [SerializeField] GameObject sfxCredits;
        [SerializeField] Image levelImage;
        [SerializeField] TMP_Text levelText;
        [SerializeField] TMP_Text endlessDropScore;
        [SerializeField] TMP_Text endlessMergeScore;

    private void Start()
    {
        for (int i = 0; i < buttonSettings.Count; i++)
        {
            Setting enumValue = (Setting)i;
            buttonSettings[i].button.onClick.AddListener(() => LoadWithSetting(enumValue));
        }

        leftArrow.onClick.AddListener(Decrement);
        rightArrow.onClick.AddListener(Increment);
        clearData.onClick.AddListener(ResetData);
        sfxButton.onClick.AddListener(Credits);
        sfxCredits.SetActive(false);
        DisplayLevel();
    }

    void Increment()
    {
        PlaySound(0);
        levelToLoad = (levelToLoad == listOfLevels.Count - 1) ? 0 : levelToLoad + 1;
        DisplayLevel();
    }

    void Decrement()
    {
        PlaySound(0);
        levelToLoad = (levelToLoad <= 0 ) ? listOfLevels.Count - 1 : levelToLoad - 1;
        DisplayLevel();
    }

    void PlaySound(int index)
    {
        AudioManager.instance.PlaySound(menuSound, 0.4f);
    }

    void LoadWithSetting(Setting setting)
    {
        Application.targetFrameRate = 60;
        PrefManager.SetLevel(listOfLevels[levelToLoad].levelName);
        PrefManager.SetSetting(setting);
        SceneManager.LoadScene(listOfLevels[levelToLoad].levelName.ToString());
    }

    void DisplayLevel()
    {
        LevelInfo currentLevel = listOfLevels[levelToLoad];
        levelText.text = AutoTranslate.DoEnum(currentLevel.levelName);
        levelImage.sprite = currentLevel.sprite;

        foreach (ButtonInfo BI in buttonSettings)
        {
            BI.button.enabled = true;
            BI.image.color = Color.white;
        }

        int score = PrefManager.GetScore(currentLevel.levelName, Setting.MergeCrown);
        buttonSettings[0].image.color = (score >= 1) ? Color.yellow : Color.white;
        buttonSettings[0].achievement.SetActive(score >= 50);

        if (!PlayerPrefs.HasKey($"{currentLevel.levelName} - {Setting.DropShape}")) PrefManager.SetScore(currentLevel.levelName, Setting.DropShape, 1000);
        score = PrefManager.GetScore(currentLevel.levelName, Setting.DropShape);
        buttonSettings[1].image.color = (score <= 999) ? Color.yellow : Color.white;
        buttonSettings[1].achievement.SetActive(score <= 200);

        score = PrefManager.GetScore(currentLevel.levelName, Setting.DropEndless);
        endlessDropScore.text = AutoTranslate.High_Score(score.ToString());

        score = PrefManager.GetScore(currentLevel.levelName, Setting.MergeEndless);
        endlessMergeScore.text = AutoTranslate.High_Score(score.ToString());
    }

    void ResetData()
    {
        for (int i = 0; i < listOfLevels.Count; i++)
        {
            LevelInfo currentLevel = listOfLevels[i];
            PrefManager.SetScore(currentLevel.levelName, Setting.MergeCrown, 0);
            PrefManager.SetScore(currentLevel.levelName, Setting.DropShape, 1000);
            PrefManager.SetScore(currentLevel.levelName, Setting.MergeEndless, 0);
            PrefManager.SetScore(currentLevel.levelName, Setting.DropEndless, 0);
        }

        PlaySound(0);
        DisplayLevel();
    }

    void Credits()
    {
        PlaySound(0);
        if (sfxCredits.activeSelf)
            sfxCredits.SetActive(false);
        else
            sfxCredits.SetActive(true);
    }
}
