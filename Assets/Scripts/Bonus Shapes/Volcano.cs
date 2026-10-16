using System.Collections.Generic;
using UnityEngine;

public class Volcano : Shape
{
    protected override void Setup()
    {
        EventManager.inst.Subscribe<CreatedShape>(OnNewShape);
    }
    void OnNewShape(CreatedShape info)
    {
        if (info.newShape is Star)
        {
            float ySpawn = ShapeManager.dropState == ColumnDrop.Top ? ShapeManager.inst.YSpawnRange().highest : ShapeManager.inst.YSpawnRange().lowest; 
            ShapeManager.inst.GenerateShape(typeof(Circle).Name, new(this.transform.position.x, ySpawn), CreationType.Special);
        }        
    }
    public override void OnReturn(ReturnType returnType)
    {
        EventManager.inst.Unsubscribe<CreatedShape>(OnNewShape);
    }
}