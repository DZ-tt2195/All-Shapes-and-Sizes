using System.Collections.Generic;
using UnityEngine;

public class Volcano : Shape
{
    protected override void HitOtherShape(Shape otherShape)
    {
        HashSet<Shape> allCopies = ShapeManager.inst.GetExistingShapes()[otherShape.GetType().Name];
        foreach (Shape next in new HashSet<Shape>(allCopies))
            ShapeManager.inst.ReturnShape(next, ReturnType.Penalty);
        ShapeManager.inst.ReturnShape(this, ReturnType.Done);
        ShapeManager.inst.GenerateShape(typeof(Star).Name, this.transform.position, CreationType.Special);
    }
    /*
    [SerializeField] int starting;
    int currentCount;
    protected override void Setup()
    {
        currentCount = starting;
        this.textBox.text = $"{currentCount}";
        EventManager.inst.Subscribe<CreatedShape>(OnNewShape);
    }
    void OnNewShape(CreatedShape info)
    {
        if (info.type == CreationType.Combine && info.newShape is Diamond)
        {
            ShapeManager.inst.GenerateShape(typeof(Square).Name, this.transform.position, CreationType.Special);
            currentCount--;
            this.textBox.text = $"{currentCount}";
            if (currentCount == 0)
                ShapeManager.inst.ReturnShape(this, ReturnType.Done);
        }        
    }
    public override void OnReturn(ReturnType returnType)
    {
        EventManager.inst.Unsubscribe<CreatedShape>(OnNewShape);
    }
    */
}