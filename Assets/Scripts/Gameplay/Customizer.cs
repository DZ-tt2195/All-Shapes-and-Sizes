using UnityEngine;
using TMPro;
using MyBox;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class Customizer : MonoBehaviour
{
    public static Customizer inst;

    [Foldout("UI", true)]
        [SerializeField] Image tutorialBackground; public Image GetBackground => tutorialBackground;
        [SerializeField] Button guideButton;
        [SerializeField] Button confirmButton;

    [Foldout("Texts", true)]
        [SerializeField] TMP_Text guideText;
        [SerializeField] TMP_Text confirmText;
        [SerializeField] TMP_Text bonusShapes;

    [Foldout("Customize", true)]
        [SerializeField] ShapeDisplay displayPrefab;
        [SerializeField] Transform storeShapes;
        [SerializeField] List<ShapeDisplay> displaysOnScreen;
        HashSet<Shape> currentBonusShapes = new();

    void Awake()
    {
        inst = this;
        guideText.text = AutoTranslate.Open_Guide();
        bonusShapes.text = AutoTranslate.Choose_Bonuses(displaysOnScreen.Count.ToString());
        confirmText.text = AutoTranslate.Confirm();

        foreach (ShapeDisplay display in displaysOnScreen)
            display.gameObject.SetActive(false);

        guideButton.onClick.AddListener(ClickGuide);
        void ClickGuide()
        {
            if (tutorialBackground.gameObject.activeSelf)
            {
                tutorialBackground.gameObject.SetActive(false);
                guideText.text = AutoTranslate.Open_Guide();
            }
            else
            {
                tutorialBackground.gameObject.SetActive(true);
                guideText.text = AutoTranslate.Close_Guide();                
            }
        }
        ChooseBonuses();
    }
    void ChooseBonuses()
    {
        List<Shape> allBonuses = GameFiles.inst.AllBonuses();
        foreach (Shape shape in allBonuses)
        {
            ShapeDisplay nextDisplay = Instantiate(displayPrefab, storeShapes);
            nextDisplay.AssignShape(shape);
            nextDisplay.toggle.isOn = false;
            nextDisplay.toggle.onValueChanged.AddListener(ShapeToggle);

            void ShapeToggle(bool enabled)
            {
                if (enabled)
                {
                    currentBonusShapes.Add(shape);
                    if (currentBonusShapes.Count > displaysOnScreen.Count)
                        nextDisplay.toggle.isOn = false;
                    else
                        AudioManager.instance.Menu();
                }
                else
                {
                    AudioManager.instance.Menu();
                    currentBonusShapes.Remove(shape);
                }
            }
        }        

        confirmButton.onClick.AddListener(Done);

        void Done()
        {
            bonusShapes.gameObject.SetActive(false);
            storeShapes.transform.parent.gameObject.SetActive(false);
            confirmButton.gameObject.SetActive(false);
            guideButton.gameObject.SetActive(true);

            while (currentBonusShapes.Count < displaysOnScreen.Count)
            {
                int randomNumber = Random.Range(0, allBonuses.Count);
                currentBonusShapes.Add(allBonuses[randomNumber]);                
            }

            List<Shape> selectedShapes = currentBonusShapes.ToList();
            for (int i = 0; i<selectedShapes.Count; i++)
                displaysOnScreen[i].AssignShape(selectedShapes[i]);

            ShapeManager.instance.ChosenShapes(currentBonusShapes);
            tutorialBackground.gameObject.SetActive(false);
        }
    }
}
