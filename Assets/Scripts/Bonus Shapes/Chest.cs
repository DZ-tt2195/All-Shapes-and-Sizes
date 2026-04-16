using System;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Shape
{
    [SerializeField] int maxUpgrade;
    int currentCount;
    public override void Setup(Vector2 start, bool cursed)
    {
        base.Setup(start, cursed);
        currentCount = maxUpgrade;
        textBox.text = currentCount.ToString();
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(100, 85) : new(60, 50);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        if (otherShape is Square)
        {
            currentCount--;
            textBox.text = currentCount.ToString();
            otherShape.ScoreShapes(null, typeof(Arrow).Name);
            if (currentCount == 0)
                ShapeManager.instance.ReturnShape(this);
        }
    }
}
