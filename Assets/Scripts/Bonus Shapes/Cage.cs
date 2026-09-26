using UnityEngine;
using TMPro;

public class Cage : Shape
{
    int currentCount;
    [SerializeField] int requirement;
    public override void Setup(Vector2 start, bool cursed)
    {
        base.Setup(start, cursed);
        currentCount = 0;
        this.textBox.text = $"{currentCount}";
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 75) : new(60, 50);
    }
    public override bool TrackNewShapes() => true;
    public override void OnNewShape(Shape newShape, CreationType creationType)
    {
        if (creationType == CreationType.Combine)
        {
            currentCount++;
            if (currentCount >= requirement && this.HasAbility())
            {
                ShapeManager.inst.ReturnShape(this);                
                ShapeManager.inst.GenerateShape(typeof(Circle).Name, this.transform.position, CreationType.Other);
                ShapeManager.inst.GenerateShape(typeof(Square).Name, this.transform.position, CreationType.Other);
                ShapeManager.inst.GenerateShape(typeof(Arrow).Name, this.transform.position, CreationType.Other);
            }
        }
        else if (creationType == CreationType.Drop)
        {
            currentCount = 0;
        }
        this.textBox.text = $"{currentCount}";
    }
}