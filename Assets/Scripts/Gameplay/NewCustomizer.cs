using UnityEngine;
using TMPro;
using MyBox;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class NewCustomizer : MonoBehaviour
{
    public static int numBonusShapes = 3;
    [Foldout("UI", true)]
        [SerializeField] Button openCustomizer;
        [SerializeField] TMP_Text customizerText;
        [SerializeField] Transform customizerScreen;
        [SerializeField] Button confirmButton;
        [SerializeField] TMP_Text confirmText;
        [SerializeField] TMP_Text bonusShapes;

    [Foldout("Customize", true)]
        [SerializeField] ShapeDisplay displayPrefab;
        [SerializeField] Transform storeShapes;
        List<int> currentBonusShapes = new();

    void Awake()
    {
        bonusShapes.text = AutoTranslate.Choose_Bonuses(numBonusShapes.ToString());
        confirmText.text = AutoTranslate.Confirm();
        customizerText.text = AutoTranslate.Open_Customizer();
        customizerScreen.gameObject.SetActive(false);
        openCustomizer.onClick.AddListener(OpenCustomizer);
        void OpenCustomizer()
        {
            AudioManager.instance.Menu();
            customizerScreen.gameObject.SetActive(true);
        }

        List<Shape> allBonuses = GameFiles.inst.AllBonuses();
        for (int i = 0; i<allBonuses.Count; i++)
        {
            ShapeDisplay nextDisplay = Instantiate(displayPrefab, storeShapes);
            nextDisplay.AssignShape(allBonuses[i]);
            int number = i;

            nextDisplay.toggle.onValueChanged.AddListener(ShapeToggle);
            if (AlreadySaved(number))
            {
                nextDisplay.toggle.isOn = true;
                currentBonusShapes.Add(number);
            }
            else
            {
                nextDisplay.toggle.isOn = false;
            }

            void ShapeToggle(bool enabled)
            {
                if (enabled)
                {
                    currentBonusShapes.Add(number);
                    if (currentBonusShapes.Count > numBonusShapes)
                        nextDisplay.toggle.isOn = false;
                    else
                        AudioManager.instance.Menu();
                }
                else
                {
                    AudioManager.instance.Menu();
                    currentBonusShapes.Remove(number);
                }
            }
        }        

        confirmButton.onClick.AddListener(Done);
        void Done()
        {
            AudioManager.instance.Menu();
            for (int i = 0; i<numBonusShapes; i++)
            {
                if (i < currentBonusShapes.Count)
                    PrefManager.SetShape(i, currentBonusShapes[i]);
                else
                    PrefManager.SetShape(i, -1);
            }
            PlayerPrefs.Save();
            customizerScreen.gameObject.SetActive(false);
        }
    }
    bool AlreadySaved(int num)
    {
        for (int i = 0; i<numBonusShapes; i++)
        {
            if (PrefManager.GetShape(i) == num)
                return true;
        }
        return false;
    }
}
