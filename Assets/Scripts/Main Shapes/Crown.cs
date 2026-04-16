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
            ShapeManager.instance.mergedCrowns = true;
            ScoreShapes(otherShape, "");
            if (PrefManager.GetSetting() == Setting.Merge_Crown)
                ShapeManager.instance.GameOver(AutoTranslate.You_Won());
        }
    }
}
