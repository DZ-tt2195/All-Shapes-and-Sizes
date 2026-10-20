using System.Collections.Generic;
using UnityEngine;

public class Phoenix : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(110, 80) : new(70, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        HashSet<Shape> allCopies = ShapeManager.inst.GetExistingShapes()[otherShape.GetType().Name];
        foreach (Shape next in new HashSet<Shape>(allCopies))
            ShapeManager.inst.ReturnShape(next, ReturnType.Penalty);
        ShapeManager.inst.ReturnShape(this, ReturnType.Done);
        ShapeManager.inst.GenerateShape(typeof(Star).Name, this.transform.position, CreationType.Special);
    }
}