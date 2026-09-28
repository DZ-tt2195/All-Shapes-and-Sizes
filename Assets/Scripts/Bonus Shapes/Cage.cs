using UnityEngine;
using System;
using System.Collections.Generic;

public class Cage : Shape
{
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 75) : new(60, 50);
    }
    public override void OnReturn(ReturnType returnType)
    {
        if (returnType == ReturnType.Destroy)
        {
            List<Type> toDrop = new() {typeof(Circle), typeof(Square), typeof(Arrow)};
            ShapeManager.inst.StartCoroutine(ShapeManager.inst.DropRandomly(toDrop, this.transform.position, 0.1f, 0.1f, false));
        }
    }
}
