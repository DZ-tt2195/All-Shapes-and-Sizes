using UnityEngine;

public class Bomb : Shape
{
    [SerializeField] AudioClip bombSound;
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(55, 95) : new(45, 80);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        canInteract = false;
        otherShape.canInteract = false;

        AudioManager.instance.PlaySound(bombSound, 0.3f);
        ShapeManager.instance.ReturnShape(otherShape);
        ShapeManager.instance.ReturnShape(this);
    }
}
