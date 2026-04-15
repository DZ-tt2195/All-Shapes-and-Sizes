using UnityEngine;

public class Duplicate : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 90) : new(60, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        if (otherShape.IsMainShape())
        {
            otherShape.CursedStatus(true);
            ShapeManager.instance.ReturnShape(this);
            ShapeManager.instance.GenerateShape(otherShape.GetType().Name, this.transform.position, CreationType.Drop, true);
        }
    }
}
