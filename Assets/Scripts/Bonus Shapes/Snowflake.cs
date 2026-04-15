using System.Collections.Generic;
using UnityEngine;

public class Snowflake : Shape
{
    int disappearOn;
    [SerializeField] int increment;
    [SerializeField] AudioClip dingSound;
    public override void Setup(Vector2 start, bool cursed)
    {
        disappearOn = ShapeManager.instance.merged + increment;
        base.Setup(start, cursed);
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 90) : new(60, 60);
    }
    void Update()
    {
        int currentCount = disappearOn - ShapeManager.instance.merged;
        this.textBox.text = $"{currentCount}";
        if (currentCount <= 0)
        {
            ShapeManager.instance.ReturnShape(this);
            AudioManager.instance.PlaySound(dingSound, 0.3f);
        }
    }
}
