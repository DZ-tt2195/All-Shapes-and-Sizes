using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyBox;
public class ShapeDisplay : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text descriptionText;
    public Toggle toggle;

    public void AssignShape(Shape shape)
    {
        this.gameObject.SetActive(true);
        ShapeManager.ApplySprite(image, shape, false);
        descriptionText.text = Translator.inst.Translate($"{shape.name}");
    }
}
