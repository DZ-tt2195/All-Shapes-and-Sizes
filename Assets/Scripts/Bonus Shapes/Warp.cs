using UnityEngine;

public class Warp : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 90) : new(60, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        canInteract = false;
        otherShape.canInteract = false;

        float newXPosition = otherShape.transform.position.x > 0 ? ShapeManager.instance.leftWall.transform.position.x + 1f : ShapeManager.instance.rightWall.transform.position.x - 1f;
        ShapeManager.instance.GenerateShape(otherShape.GetType().Name, new(newXPosition, this.transform.position.y));
        ShapeManager.instance.ReturnShape(otherShape);
        ShapeManager.instance.ReturnShape(this);
    }
}
