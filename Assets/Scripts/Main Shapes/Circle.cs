using UnityEngine;

public class Circle : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(50, 50) : new(40, 40);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        if (otherShape is Circle)
        {
            this.canInteract = false;
            otherShape.canInteract = false;
            Upgrade(otherShape);
        }
    }
    protected override void Upgrade(Shape otherShape)
    {
        ShapeManager.instance.GenerateShape(typeof(Square).Name, this.transform.position, CreationType.Merge);
        ScoreShapes(otherShape);
    }
}
