using System;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Shape
{
    [SerializeField] int maxUpgrade;
    int currentCount;
    public override string MyText() => AutoTranslate.Chest(maxUpgrade.ToString());
    protected override void Setup()
    {
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
            ShapeManager.inst.ReturnShape(otherShape, ReturnType.Done);
            ShapeManager.inst.GenerateShape(typeof(Arrow).Name, otherShape.transform.position, CreationType.Special);
            if (currentCount == 0)
                ShapeManager.inst.ReturnShape(this, ReturnType.Done);
        }
        else if (otherShape is Circle)
        {
            ShapeManager.inst.ReturnShape(otherShape, ReturnType.Penalty);
        }
    }
}