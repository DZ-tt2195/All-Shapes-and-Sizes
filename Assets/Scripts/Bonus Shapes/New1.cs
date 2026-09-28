using System.Collections.Generic;
using UnityEngine;

public class New1 : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(70, 90) : new(50, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        HashSet<Shape> currentArrows = ShapeManager.inst.GetExistingShapes()[typeof(Arrow).Name];
        List<Vector2> arrowPositions = new();
        foreach (Shape arrow in new HashSet<Shape>(currentArrows))
        {
            arrowPositions.Add(arrow.transform.position);
            ShapeManager.inst.ReturnShape(arrow);
        }

        HashSet<Shape> currentDiamonds = ShapeManager.inst.GetExistingShapes()[typeof(Diamond).Name];
        List<Vector2> diamondPositions = new();
        foreach (Shape diamond in new HashSet<Shape>(currentDiamonds))
        {
            diamondPositions.Add(diamond.transform.position);
            ShapeManager.inst.ReturnShape(diamond);
        }

        foreach (Vector2 newDiamond in arrowPositions)
            ShapeManager.inst.GenerateShape(typeof(Diamond).Name, newDiamond, CreationType.Other);
        foreach (Vector2 newArrow in diamondPositions)
            ShapeManager.inst.GenerateShape(typeof(Arrow).Name, newArrow, CreationType.Other);

        ShapeManager.inst.ReturnShape(this);
    }
}