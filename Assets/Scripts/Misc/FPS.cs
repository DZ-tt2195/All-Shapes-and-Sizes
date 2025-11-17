using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS : MonoBehaviour
{
    TMP_Text textbox;
    int lastframe = 0;
    int lastupdate = 60;
    float[] framearray = new float[60];

    private void Awake()
    {
        textbox = GetComponent<TMP_Text>();
    }

    void Update()
    {
        textbox.text = Translator.inst.Translate("FPS", new() { ("Num", CalculateFrames().ToString()) });
    }

    int CalculateFrames()
    {
        framearray[lastframe] = Time.deltaTime;
        lastframe = (lastframe + 1);
        if (lastframe == 60)
        {
            lastframe = 0;
            float total = 0;
            for (int i = 0; i < framearray.Length; i++)
                total += framearray[i];
            lastupdate = (int)(framearray.Length / total);
            return lastupdate;
        }
        return (lastupdate > Application.targetFrameRate) ? Application.targetFrameRate : lastupdate;
    }
}
