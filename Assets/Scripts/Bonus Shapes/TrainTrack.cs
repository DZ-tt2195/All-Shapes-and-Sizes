using UnityEngine;

public class TrainTrack : Shape
{
    [SerializeField] AudioClip warpSound;
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(110, 80) : new(65, 50);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        AudioManager.instance.PlaySound(warpSound, 0.3f);
        ShapeManager.inst.ReturnShape(this, ReturnType.Done);

        (float leftSpawn, float rightSpawn) = ShapeManager.inst.XSpawnRange();
        float newXPosition = otherShape.transform.position.x > 0 ? leftSpawn : rightSpawn;
        otherShape.PopUp(new(newXPosition, otherShape.transform.position.y), otherShape.cursed, false);
    }
}