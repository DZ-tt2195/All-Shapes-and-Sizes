using UnityEngine;

public class Mirror : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(55, 90) : new(50, 80);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        if (otherShape.IsMainShape())
        {
            otherShape.CursedStatus(true);
            ShapeManager.inst.ReturnShape(this);
            ShapeManager.inst.GenerateShape(otherShape.GetType().Name, this.transform.position, CreationType.Other, true);
        }
    }
}