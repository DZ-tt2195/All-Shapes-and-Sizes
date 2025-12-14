using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TakeScreenshot : EditorWindow
{
    [MenuItem("Tools/Take Screenshot")]
    static void Picture()
    {
        string sceneName = EditorSceneManager.GetActiveScene().name;
        ScreenCapture.CaptureScreenshot($"Assets/Resources/{sceneName}.png");
        AssetDatabase.Refresh();
    }
}
