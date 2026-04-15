using UnityEngine;

public class Heart : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(105, 90) : new (70, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        if (otherShape is Heart)
        {
            Upgrade(otherShape);
        }
    }
    protected override void Upgrade(Shape otherShape)
    {
        ShapeManager.instance.GenerateShape(typeof(Crown).Name, this.transform.position, CreationType.Merge);
        ScoreShapes(otherShape);
    }
}
