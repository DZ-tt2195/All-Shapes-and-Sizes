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
            ScoreShapes(otherShape, typeof(Heart).Name);
    }
}
