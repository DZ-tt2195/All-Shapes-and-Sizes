using System.Collections.Generic;
using UnityEngine;

public class Snowflake : Shape
{
    int disappearOn;
    [SerializeField] int increment;
    [SerializeField] AudioClip dingSound;
    public override void Setup(Vector2 start, bool cursed)
    {
        base.Setup(start, cursed);
        disappearOn = increment;
        this.textBox.text = $"{disappearOn}";
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 90) : new(60, 60);
    }
    public override bool TrackNewShapes() => true;
    public override void OnNewShape(Shape newShape, CreationType creationType)
    {
        if (creationType == CreationType.Combine)
        {
            disappearOn--;
            this.textBox.text = $"{disappearOn}";
            if (disappearOn == 0)
            {
                ShapeManager.inst.ReturnShape(this);
                AudioManager.instance.PlaySound(dingSound, 0.3f);                
            }
        }
    }
}