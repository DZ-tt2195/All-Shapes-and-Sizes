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
        canInteract = false;
        otherShape.canInteract = false;
        AudioManager.instance.PlaySound(warpSound, 0.3f);

        float newXPosition = otherShape.transform.position.x > 0 ? ShapeManager.instance.leftWall.transform.position.x + 1f : ShapeManager.instance.rightWall.transform.position.x - 1f;
        ShapeManager.instance.GenerateShape(otherShape.GetType().Name, new(newXPosition, this.transform.position.y), CreationType.Drop);
        ShapeManager.instance.ReturnShape(otherShape);
        ShapeManager.instance.ReturnShape(this);
    }
}
