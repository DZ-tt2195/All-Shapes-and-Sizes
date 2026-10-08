using UnityEngine;
using System.Collections;

public class Inverter : Shape
{
    [SerializeField] AudioClip gravitySound;
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 90) : new(60, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        AudioManager.instance.PlaySound(gravitySound, 0.5f);
        ShapeManager.inst.ChangeGravity(ShapeManager.dropState == ColumnDrop.Top ? ColumnDrop.Bottom : ColumnDrop.Top);
        ShapeManager.inst.ReturnShape(this, ReturnType.Done);
    }
}