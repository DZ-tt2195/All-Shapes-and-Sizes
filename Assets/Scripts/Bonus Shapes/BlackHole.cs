using System;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : Shape
{
    [SerializeField] int maxUpgrade;
    int currentCount;
    public override string MyText() => AutoTranslate.BlackHole(maxUpgrade.ToString());
    protected override void Setup()
    {
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
            ShapeManager.inst.ReturnShape(otherShape, ReturnType.Done);
            ShapeManager.inst.GenerateShape(typeof(Square).Name, otherShape.transform.position, CreationType.Special, true);
            if (currentCount == 0)
                ShapeManager.inst.ReturnShape(this, ReturnType.Done);
        }
    }
}