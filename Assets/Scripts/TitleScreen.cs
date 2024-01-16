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
        [SerializeField] TMP_Dropdown fpsSetting;
        [SerializeField] Button sfxButton;
        [SerializeField] Button clearData;
        [SerializeField] Button leftArrow;
        [SerializeField] Button rightArrow;

    [Foldout("Text and images", true)]
        [SerializeField] GameObject sfxCredits;
        [SerializeField] Image levelImage;
        [SerializeField] TMP_Text levelText;
        [SerializeField] TMP_Text maxDropScore;
        [SerializeField] TMP_Text endlessHighScore;
        public enum Setting { MergeCrown, Drops, MaxDrop, Endless };

    private void Start()
    {
        Debug.Log(Screen.currentResolution);

        levelToLoad = LevelSettings.instance.lastLevel;
        fpsSetting.value = (Application.targetFrameRate == 60) ? 0 : 1;
        fpsSetting.onValueChanged.AddListener(PlaySound);

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
        Application.targetFrameRate = fpsSetting.value == 0 ? 60 : 30;
        LevelSettings.instance.setting = setting;
        LevelSettings.instance.lastLevel = levelToLoad;
        SceneManager.LoadScene(listOfLevels[levelToLoad].name);
    }

    void DisplayLevel()
    {
        levelText.text = listOfLevels[levelToLoad].name;
        levelImage.sprite = listOfLevels[levelToLoad].sprite;

        foreach (ButtonInfo BI in buttonSettings)
        {
            BI.button.enabled = true;
            BI.image.color = Color.white;
        }

        int score = PlayerPrefs.GetInt($"{listOfLevels[levelToLoad].name} - Merge");
        buttonSettings[0].image.color = (score >= 1) ? Color.yellow : Color.white;
        buttonSettings[0].achievement.SetActive(score >= 50);

        if (!PlayerPrefs.HasKey($"{listOfLevels[levelToLoad].name} - Drops"))
            PlayerPrefs.SetInt($"{listOfLevels[levelToLoad].name} - Drops", 1000);
        score = PlayerPrefs.GetInt($"{listOfLevels[levelToLoad].name} - Drops");
        buttonSettings[1].image.color = (score <= 450) ? Color.yellow : Color.white;
        buttonSettings[1].achievement.SetActive(score <= 450);

        score = PlayerPrefs.GetInt($"{listOfLevels[levelToLoad].name} - MaxDrop");
        maxDropScore.text = $"High Score:\nDropped {score}";

        score = PlayerPrefs.GetInt($"{listOfLevels[levelToLoad].name} - Endless");
        endlessHighScore.text = $"High Score:\n{score} Points";
    }

    void ResetData()
    {
        for (int i = 0; i < listOfLevels.Count; i++)
        {
            PlayerPrefs.SetInt($"{listOfLevels[i].name} - Merge", 0);
            PlayerPrefs.SetInt($"{listOfLevels[i].name} - Drops", 1000);
            PlayerPrefs.SetInt($"{listOfLevels[i].name} - MaxDrop", 0);
            PlayerPrefs.SetInt($"{listOfLevels[i].name} - Endless", 0);
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
