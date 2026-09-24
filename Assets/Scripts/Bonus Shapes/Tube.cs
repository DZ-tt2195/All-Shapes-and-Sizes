using UnityEngine;

public class Tube : Shape
{
    [SerializeField] AudioClip warpSound;
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(55, 90) : new(45, 70);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        AudioManager.instance.PlaySound(warpSound, 0.3f);

        (float bottomSpawn, float topSpawn) = ShapeManager.inst.YSpawnRange();
        float newYPosition = ShapeManager.dropState == ColumnDrop.Top ? bottomSpawn : topSpawn;
        ShapeManager.inst.GenerateShape(otherShape.GetType().Name, new(otherShape.transform.position.x, newYPosition), CreationType.Drop);
        
        ShapeManager.inst.ReturnShape(otherShape);
        ShapeManager.inst.ReturnShape(this);
    }
}
