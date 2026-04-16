using UnityEngine;

public class Multiply : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(100, 100) : new(60, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        if (otherShape is Multiply)
            ScoreShapes(otherShape, typeof(Star).Name);
    }
}
