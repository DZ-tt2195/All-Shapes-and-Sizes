using UnityEngine;

public class Arrow : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(100, 100) : new(70, 70);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        if (otherShape is Arrow)
        {
            Upgrade(otherShape);
        }
    }
    protected override void Upgrade(Shape otherShape)
    {
        ScoreShapes(otherShape, typeof(Diamond).Name);
    }
}
