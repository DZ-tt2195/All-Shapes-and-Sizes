using UnityEngine;

public class Bomb : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(55, 95) : new(45, 80);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        canInteract = false;
        otherShape.canInteract = false;

        AudioManager.instance.PlaySound(ShapeManager.instance.bombSound, 0.35f);
        ShapeManager.instance.ReturnShape(otherShape);
        ShapeManager.instance.ReturnShape(this);
    }
}
