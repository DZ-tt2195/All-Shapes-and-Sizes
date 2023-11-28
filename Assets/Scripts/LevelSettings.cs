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
    [ReadOnly] public Setting setting;

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
}
