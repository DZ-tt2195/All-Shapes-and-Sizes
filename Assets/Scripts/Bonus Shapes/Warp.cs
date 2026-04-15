using UnityEngine;

public class Warp : Shape
{
    [SerializeField] AudioClip warpSound;
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 90) : new(60, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        AudioManager.instance.PlaySound(warpSound, 0.3f);

        (float leftSpawn, float rightSpawn) = ShapeManager.instance.XSpawnRange();
        float newXPosition = otherShape.transform.position.x > 0 ? leftSpawn : rightSpawn;
        ShapeManager.instance.GenerateShape(otherShape.GetType().Name, new(newXPosition, otherShape.transform.position.y), CreationType.Drop, otherShape.cursed);
        
        ShapeManager.instance.ReturnShape(otherShape);
        ShapeManager.instance.ReturnShape(this);
    }
}
