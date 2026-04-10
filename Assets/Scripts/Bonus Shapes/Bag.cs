using UnityEngine;

public class Bag : Shape
{
    [SerializeField] int spawnAmount;
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(70, 90) : new(50, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        this.canInteract = false;
        ShapeManager.instance.StartCoroutine(ShapeManager.instance.DropRandomly(typeof(BlueCircle), spawnAmount));
        ShapeManager.instance.ReturnShape(this);
    }
}
