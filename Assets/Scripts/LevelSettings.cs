using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using UnityEngine.SceneManagement;
using static TitleScreen;

public class LevelSettings : MonoBehaviour
{
    public static LevelSettings instance;
    [ReadOnly] public int lastLevel;
    [ReadOnly] public Setting setting;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Application.targetFrameRate = 60;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
