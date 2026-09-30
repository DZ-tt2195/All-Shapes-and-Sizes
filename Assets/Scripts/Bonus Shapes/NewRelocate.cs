using UnityEngine;

public class NewRelocate : Shape
{
    static Shape frozen;
    [SerializeField] AudioClip warpSound;
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(55, 90) : new(45, 70);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        AudioManager.instance.PlaySound(warpSound, 0.3f);

        if (frozen != null)
            frozen.GetRB.constraints = RigidbodyConstraints2D.None;

        frozen = ShapeManager.inst.GenerateShape(otherShape.GetType().Name, Vector3.zero, CreationType.Special);
        frozen.GetRB.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        
        ShapeManager.inst.ReturnShape(otherShape, ReturnType.Done);
        ShapeManager.inst.ReturnShape(this, ReturnType.Done);
    }
}