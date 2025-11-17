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
        public enum Setting { MergeCrown, DropShape, DropEndless, MergeEndless };

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
        clearData.onClick.AddListener(ResetData);
        sfxButton.onClick.AddListener(Credits);
        sfxCredits.SetActive(false);
        levelToLoad = LevelSettings.instance.lastLevel;
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
        LevelSettings.instance.setting = setting;
        LevelSettings.instance.lastLevel = levelToLoad;
        SceneManager.LoadScene(listOfLevels[levelToLoad].name);
    }

    void DisplayLevel()
    {
        LevelInfo currentLevel = listOfLevels[levelToLoad];
        levelText.text = Translator.inst.Translate(currentLevel.name);
        levelImage.sprite = currentLevel.sprite;

        foreach (ButtonInfo BI in buttonSettings)
        {
            BI.button.enabled = true;
            BI.image.color = Color.white;
        }

        int score = PlayerPrefs.GetInt($"{currentLevel.name} - {Setting.MergeCrown}");
        buttonSettings[0].image.color = (score >= 1) ? Color.yellow : Color.white;
        buttonSettings[0].achievement.SetActive(score >= 50);

        if (!PlayerPrefs.HasKey($"{currentLevel.name} - {Setting.DropShape}")) PlayerPrefs.SetInt($"{currentLevel.name} - {Setting.DropShape}", 1000);
        score = PlayerPrefs.GetInt($"{currentLevel.name} - {Setting.DropShape}");
        buttonSettings[1].image.color = (score <= 999) ? Color.yellow : Color.white;
        buttonSettings[1].achievement.SetActive(score <= 200);

        score = PlayerPrefs.GetInt($"{currentLevel.name} - {Setting.DropEndless}");
        endlessDropScore.text = Translator.inst.Translate("High Score", new() { ("Num", score.ToString()) });

        score = PlayerPrefs.GetInt($"{currentLevel.name} - {Setting.MergeEndless}");
        endlessMergeScore.text = Translator.inst.Translate("High Score", new() { ("Num", score.ToString()) });
    }

    void ResetData()
    {
        for (int i = 0; i < listOfLevels.Count; i++)
        {
            LevelInfo currentLevel = listOfLevels[i];
            PlayerPrefs.SetInt($"{currentLevel.name} - {Setting.MergeCrown}", 0);
            PlayerPrefs.SetInt($"{currentLevel.name} - {Setting.DropShape}", 1000);
            PlayerPrefs.SetInt($"{currentLevel.name} - {Setting.MergeEndless}", 0);
            PlayerPrefs.SetInt($"{currentLevel.name} - {Setting.DropEndless}", 0);
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
