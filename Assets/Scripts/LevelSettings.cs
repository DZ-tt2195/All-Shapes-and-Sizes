using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using UnityEngine.SceneManagement;

public class LevelSettings : MonoBehaviour
{
    public static LevelSettings instance;
    public enum Setting { MergeCrown, ReachScore, Endless };
    [ReadOnly] public Setting setting;
    int levelToLoad = 1;

    [SerializeField] List<Button> buttonSettings;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        for (int i = 0; i<buttonSettings.Count; i++)
        {
            Setting enumValue = (Setting)i;
            buttonSettings[i].onClick.AddListener(() => LoadWithSetting(enumValue));
        }
    }

    void LoadWithSetting(Setting setting)
    {
        this.setting = setting;
        SceneManager.LoadScene(levelToLoad);
    }
}
