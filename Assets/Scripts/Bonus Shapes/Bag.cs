using UnityEngine;

public class Bag : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 90) : new(60, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        this.canInteract = false;
        ShapeManager.instance.StartCoroutine(ShapeManager.instance.DropRandomly(typeof(BlueCircle), 5));
        ShapeManager.instance.ReturnShape(this);
    }
}
