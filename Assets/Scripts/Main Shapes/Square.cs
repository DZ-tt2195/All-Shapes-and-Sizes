using UnityEngine;

public class Square : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(65, 65) : new(50, 50);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        if (otherShape is Square)
            Upgrade(otherShape);
    }
    protected override void Upgrade(Shape otherShape)
    {
        ScoreShapes(otherShape, typeof(Arrow).Name);
    }
}
