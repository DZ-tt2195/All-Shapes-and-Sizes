using UnityEngine;

public class Inverter : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 90) : new(60, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        this.canInteract = false;
        ShapeManager.instance.ReturnShape(this);
        ShapeManager.instance.SwitchGravity();
    }
}
