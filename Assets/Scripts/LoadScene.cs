using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [Scene]
    public string scene;

    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(NextScene);
    }

    public void NextScene()
    {
        Destroy(LevelSettings.instance.gameObject);
        SceneManager.LoadScene(scene);
    }
}
