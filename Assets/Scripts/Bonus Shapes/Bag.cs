using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Bag : Shape
{
    [SerializeField] int spawnAmount;
    public override string MyText() => AutoTranslate.Bag(spawnAmount.ToString());
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(70, 90) : new(50, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        List<Type> toDrop = Enumerable.Repeat(typeof(Square), 4).ToList();
        ShapeManager.inst.StartCoroutine(ShapeManager.inst.DropRandomly(toDrop, this.transform.position, 0.1f, 0.1f, true));
        ShapeManager.inst.ReturnShape(this, ReturnType.Done);
    }
}