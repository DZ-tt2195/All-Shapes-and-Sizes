using System;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : Shape
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
        return larger ? new(90, 90) : new(60, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        if (otherShape is Circle)
        {
            currentCount--;
            textBox.text = currentCount.ToString();
            otherShape.ScoreShapes(null, typeof(Square).Name, true);
            if (currentCount == 0)
                ShapeManager.inst.ReturnShape(this);
        }
    }
}
