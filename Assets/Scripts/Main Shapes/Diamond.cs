using UnityEngine;

public class Diamond : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(110, 60) : new(80, 50);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        if (otherShape is Diamond)
        {
            this.canInteract = false;
            otherShape.canInteract = false;
            Upgrade(otherShape);
        }
    }
    protected override void Upgrade(Shape otherShape)
    {
        ShapeManager.instance.GenerateShape(typeof(Star).Name, this.transform.position, CreationType.Merge);
        ScoreShapes(otherShape);
    }
}
