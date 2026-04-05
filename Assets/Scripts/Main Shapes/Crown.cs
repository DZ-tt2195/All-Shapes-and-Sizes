using UnityEngine;

public class Crown : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(105, 90) : new (70, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        if (otherShape is Crown)
        {
            this.canInteract = false;
            otherShape.canInteract = false;
            Upgrade(otherShape);
        }
    }
    protected override void Upgrade(Shape otherShape)
    {
        ShapeManager.instance.mergedCrowns = true;
        base.Upgrade(otherShape);
        if (PrefManager.GetSetting() == Setting.MergeCrown)
            ShapeManager.instance.GameOver(AutoTranslate.You_Won());
    }
}
