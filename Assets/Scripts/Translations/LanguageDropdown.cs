using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LanguageDropdown : MonoBehaviour
{
    TMP_Dropdown dropdown;

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(ChangeLanguageDropdown);
        List<string> languages = Translator.inst.GetTranslations().Keys.ToList();
        for (int i = 0; i < languages.Count; i++)
        {
            string nextLanguage = languages[i];
            dropdown.AddOptions(new List<string>() { nextLanguage });
            if (nextLanguage.Equals(PlayerPrefs.GetString("Language")))
            {
                dropdown.value = i;
                ChangeLanguageDropdown(i);
            }
        }
        this.gameObject.SetActive(dropdown.options.Count >= 2);

        void ChangeLanguageDropdown(int n)
        {
            Translator.inst.ChangeLanguage(dropdown.options[dropdown.value].text);
        }
    }
}
