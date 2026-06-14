using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text volumeText;
    void Awake()
    {  
        volumeText.text = AutoTranslate.Volume();
        slider.value = PlayerPrefs.GetFloat("Volume");
        slider.onValueChanged.AddListener(SetLevel);
        SetLevel(PlayerPrefs.GetFloat("Volume"));

        void SetLevel(float value)
        {
            AudioManager.instance.mixer.SetFloat("Volume", (Mathf.Log10(slider.value) * 20));
            PlayerPrefs.SetFloat("Volume", slider.value);
        }                
    }
}
