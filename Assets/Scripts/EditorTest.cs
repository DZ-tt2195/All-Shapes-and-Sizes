using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EditorTest : EditorWindow
{
    [MenuItem("Tools/Take Screenshot")]
    static void TakeScreenshot()
    {
        string sceneName = EditorSceneManager.GetActiveScene().name;
        Debug.Log(sceneName);
        ScreenCapture.CaptureScreenshot($"Assets/Resources/{sceneName}.png");
    }
}
