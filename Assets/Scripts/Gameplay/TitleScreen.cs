using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;
using UnityEngine.SceneManagement;
[System.Serializable]
public class LevelButton
{
    public Button button;
    public TMP_Text settingName;
    public TMP_Text highScoreText;
    public Setting setting;
}

public enum Setting { Merge_Crown, Endless };

public class TitleScreen : MonoBehaviour
{
    [Foldout("Sound effects", true)]
        [SerializeField] AudioClip menuSound;
        [SerializeField] Slider volumeSlider;

    [Foldout("Buttons", true)]
        [SerializeField] Button sfxButton;
        [SerializeField] Button clearData;
        [SerializeField] GameObject sfxCredits;
        [SerializeField] List<LevelButton> allLevelButtons = new();

    [Foldout("Text and images", true)]
        [SerializeField] TMP_Text volume;
        [SerializeField] TMP_Text gameDesigner;
        [SerializeField] TMP_Text lastUpdate;
        [SerializeField] TMP_Text inspiration;
        [SerializeField] TMP_Text clearDataText;
        [SerializeField] TMP_Text mergeCrowns;
        [SerializeField] TMP_Text endlessScoring;
        [SerializeField] TMP_Text soundCreditsText;
        [SerializeField] TMP_Text tutorial;

    private void Start()
    {
        gameDesigner.text = AutoTranslate.Designer();
        lastUpdate.text = AutoTranslate.Last_Update();
        inspiration.text = AutoTranslate.Inspiration();
        clearDataText.text = AutoTranslate.Clear_Data();
        soundCreditsText.text = AutoTranslate.Sound_Credits();
        volume.text = AutoTranslate.Volume();
        tutorial.text = AutoTranslate.Tutorial_Text();

        clearData.onClick.AddListener(ResetData);
        sfxButton.onClick.AddListener(Credits);
        sfxCredits.SetActive(false);

        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        volumeSlider.onValueChanged.AddListener(SetLevel);
        SetLevel(PlayerPrefs.GetFloat("Volume"));

        void SetLevel(float value)
        {
            AudioManager.instance.mixer.SetFloat("Volume", (Mathf.Log10(volumeSlider.value) * 20));
            PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        }

        foreach (LevelButton thing in allLevelButtons)
        {
            thing.button.onClick.AddListener(() => LoadWithSetting(thing.setting));
            void LoadWithSetting(Setting setting)
            {
                PrefManager.SetSetting(setting);
                SceneManager.LoadScene("1. Level");
            }
        }
        DisplayScores();
    }
    void PlaySound()
    {
        AudioManager.instance.PlaySound(menuSound, 0.4f);
    }
    void DisplayScores()
    {
        foreach (LevelButton thing in allLevelButtons)
        {
            thing.settingName.text = Translator.inst.Translate(thing.setting.ToString());
            thing.highScoreText.text = AutoTranslate.High_Score(PrefManager.GetScore(thing.setting).ToString());
        }
    }
    void ResetData()
    {
        foreach (LevelButton thing in allLevelButtons)
            PrefManager.SetScore(thing.setting, 0);
        PlaySound();
        DisplayScores();
    }
    void Credits()
    {
        PlaySound();
        if (sfxCredits.activeSelf)
            sfxCredits.SetActive(false);
        else
            sfxCredits.SetActive(true);
    }
}
