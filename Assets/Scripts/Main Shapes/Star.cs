using UnityEngine;

public class Star : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(100, 100) : new(60, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        if (otherShape is Star)
        {
            this.canInteract = false;
            otherShape.canInteract = false;
            Upgrade(otherShape);
        }
    }
    protected override void Upgrade(Shape otherShape)
    {
        ShapeManager.instance.GenerateShape(typeof(Hexagon).Name, this.transform.position);
        base.Upgrade(otherShape);
    }
}
