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
    public GameMode mode;
}

public enum GameMode { Merge_Crown, Endless };

public class TitleScreen : MonoBehaviour
{
    [Foldout("Buttons", true)]
        [SerializeField] Button sfxButton;
        [SerializeField] Button clearData;
        [SerializeField] GameObject sfxCredits;
        [SerializeField] List<LevelButton> allLevelButtons = new();
        [SerializeField] Button extrasButton;
        [SerializeField] Transform extrasScreen;

    [Foldout("Text and images", true)]
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
        tutorial.text = AutoTranslate.Tutorial_Text();
        extrasButton.GetComponentInChildren<TMP_Text>().text = AutoTranslate.Extras();

        clearData.onClick.AddListener(ResetData);
        sfxButton.onClick.AddListener(Credits);
        sfxCredits.SetActive(false);
        extrasButton.onClick.AddListener(OpenExtras);

        void OpenExtras()
        {
            extrasButton.gameObject.SetActive(false);
            extrasScreen.gameObject.SetActive(true);
        }

        foreach (LevelButton thing in allLevelButtons)
        {
            thing.button.onClick.AddListener(() => LoadWithSetting(thing.mode));
            void LoadWithSetting(GameMode setting)
            {
                PrefManager.SetMode(setting);
                SceneManager.LoadScene("1. Level");
            }
        }
        DisplayScores();
    }
    void DisplayScores()
    {
        foreach (LevelButton thing in allLevelButtons)
        {
            GameMode mode = thing.mode;
            thing.settingName.text = Translator.inst.Translate(mode.ToString());
            if (PrefManager.GetScore(mode) < 0)
                thing.highScoreText.text = AutoTranslate.No_Score();
            else
                thing.highScoreText.text = Translator.inst.Translate($"{mode}_Score", new(){("Num", PrefManager.GetScore(mode).ToString())});
        }
    }
    void ResetData()
    {
        foreach (LevelButton thing in allLevelButtons)
            PlayerPrefs.DeleteKey(thing.mode.ToString());
        AudioManager.instance.Menu();
        PlayerPrefs.Save();
        DisplayScores();
    }
    void Credits()
    {
        AudioManager.instance.Menu();
        if (sfxCredits.activeSelf)
            sfxCredits.SetActive(false);
        else
            sfxCredits.SetActive(true);
    }
}
