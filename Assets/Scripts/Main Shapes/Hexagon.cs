using UnityEngine;

public class Hexagon : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(80, 80) : new(50, 50);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        if (otherShape is Hexagon)
        {
            this.canInteract = false;
            otherShape.canInteract = false;
            Upgrade(otherShape);
        }
    }
    protected override void Upgrade(Shape otherShape)
    {
        ShapeManager.instance.GenerateShape(typeof(Heart).Name, this.transform.position);
        base.Upgrade(otherShape);
    }
}
