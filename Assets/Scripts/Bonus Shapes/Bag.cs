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
        for (int i = 0; i<spawnAmount; i++)
            ShapeManager.inst.GenerateShape(typeof(Circle).Name, this.transform.position, CreationType.Drop, true);
        ShapeManager.inst.ReturnShape(this);
    }
}
