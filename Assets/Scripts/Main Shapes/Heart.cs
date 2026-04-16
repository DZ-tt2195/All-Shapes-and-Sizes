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
            ScoreShapes(otherShape, typeof(Crown).Name);
    }
}
