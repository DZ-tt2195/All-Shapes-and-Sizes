using UnityEngine;
using TMPro;

public class Egg : Shape
{
    int disappearOn;
    [SerializeField] int starting;
    [SerializeField] AudioClip breakSound;
    public override void Setup(Vector2 start, bool cursed)
    {
        base.Setup(start, cursed);
        disappearOn = starting;
        textBox.text = $"{disappearOn}";
    }
    public override bool TrackNewShapes() => true;
    public override void OnNewShape(Shape newShape, CreationType creationType)
    {
        if (creationType == CreationType.Drop)
        {
            disappearOn = Mathf.Max(0, disappearOn-1);
            this.textBox.text = $"{disappearOn}";
            if (disappearOn == 0 && this.HasAbility())
            {
                ShapeManager.inst.ReturnShape(this);
                ShapeManager.inst.GenerateShape(typeof(Star).Name, this.transform.position, CreationType.Other);
            }
        }
        else if (creationType == CreationType.Combine)
        {
            AudioManager.instance.PlaySound(breakSound, 0.3f);
            ShapeManager.inst.ReturnShape(this);            
        }
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(65, 90) : new(50, 70);
    }
}