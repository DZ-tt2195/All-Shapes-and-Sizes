using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Bag : Shape
{
    [SerializeField] int spawnCircle;
    [SerializeField] int spawnSquare;
    public override string MyText() => AutoTranslate.Bag(spawnCircle.ToString(), spawnSquare.ToString());
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(70, 90) : new(50, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        List<Type> toDrop = new();
        for (int i = 0; i<spawnCircle; i++)
            toDrop.Add(typeof(Circle));
        for (int i = 0; i<spawnSquare; i++)
            toDrop.Add(typeof(Square));

        ShapeManager.inst.StartCoroutine(ShapeManager.inst.DropRandomly(toDrop, this.transform.position, 0.1f, 0.1f, true));
        ShapeManager.inst.ReturnShape(this, ReturnType.Done);
    }
}