using UnityEngine;
using TMPro;

public class Egg : Shape
{
    int disappearOn;
    [SerializeField] int increment;
    [SerializeField] AudioClip breakSound;
    public override void Setup(Vector2 start, bool cursed)
    {
        disappearOn = ShapeManager.instance.DropCount + increment;
        base.Setup(start, cursed);
    }
    void Update()
    {
        int currentCount = disappearOn - ShapeManager.instance.DropCount;
        this.textBox.text = $"{currentCount}";
        if (HasAbility())
        {
            if (ShapeManager.instance.StreakCombines >= 1)
            {
                AudioManager.instance.PlaySound(breakSound, 0.3f);
                ShapeManager.instance.ReturnShape(this);
            }
            else if (currentCount == 0)
            {
                ShapeManager.instance.GenerateShape(typeof(Star).Name, this.transform.position, CreationType.Drop);
                ShapeManager.instance.ReturnShape(this);
            }
        }
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(65, 90) : new(50, 70);
    }
}
