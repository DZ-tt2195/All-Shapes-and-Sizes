using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TakeScreenshot : EditorWindow
{
    [MenuItem("Tools/Take Screenshot")]
    static void Picture()
    {
        string sceneName = EditorSceneManager.GetActiveScene().name;
        Debug.Log(sceneName);
        ScreenCapture.CaptureScreenshot($"Assets/Resources/{sceneName}.png");
    }
}
