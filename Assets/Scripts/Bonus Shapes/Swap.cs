using System.Collections.Generic;
using UnityEngine;

public class Swap : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(110, 60) : new(65, 40);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        HashSet<Shape> currentArrows = ShapeManager.inst.GetExistingShapes()[typeof(Arrow).Name];
        List<Vector2> arrowPositions = new();
        foreach (Shape arrow in new HashSet<Shape>(currentArrows))
        {
            if (arrow.HasAbility())
            {
                arrowPositions.Add(arrow.transform.position);
                ShapeManager.inst.ReturnShape(arrow, ReturnType.Done);
            }
        }

        HashSet<Shape> currentDiamonds = ShapeManager.inst.GetExistingShapes()[typeof(Diamond).Name];
        List<Vector2> diamondPositions = new();
        foreach (Shape diamond in new HashSet<Shape>(currentDiamonds))
        {
            if (diamond.HasAbility())
            {
                diamondPositions.Add(diamond.transform.position);
                ShapeManager.inst.ReturnShape(diamond, ReturnType.Done);
            }
        }

        foreach (Vector2 newDiamond in arrowPositions)
            ShapeManager.inst.GenerateShape(typeof(Diamond).Name, newDiamond, CreationType.Special);
        foreach (Vector2 newArrow in diamondPositions)
            ShapeManager.inst.GenerateShape(typeof(Arrow).Name, newArrow, CreationType.Special);

        ShapeManager.inst.ReturnShape(this, ReturnType.Done);
    }
}