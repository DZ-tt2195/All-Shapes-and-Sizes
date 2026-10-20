using UnityEngine;
using TMPro;

public class Egg : Shape
{
    int disappearOn;
    [SerializeField] int starting;
    public override string MyText() => AutoTranslate.Egg(starting.ToString());
    protected override void Setup()
    {
        disappearOn = starting;
        textBox.text = $"{disappearOn}";
        EventManager.inst.Subscribe<CreatedShape>(OnNewShape);
    }
    void OnNewShape(CreatedShape info)
    {
        if (info.newShape == this) return;
        if (info.type == CreationType.Drop)
        {
            disappearOn = Mathf.Max(0, disappearOn-1);
            this.textBox.text = $"{disappearOn}";
            if (disappearOn == 0)
            {
                ShapeManager.inst.ReturnShape(this, ReturnType.Done);
                ShapeManager.inst.GenerateShape(typeof(Star).Name, this.transform.position, CreationType.Special);
            }
        }
        else if (info.type == CreationType.Combine)
        {
            ShapeManager.inst.ReturnShape(this, ReturnType.Penalty);            
        }
    }
    public override void OnReturn(ReturnType returnType)
    {
        EventManager.inst.Unsubscribe<CreatedShape>(OnNewShape);
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(65, 90) : new(50, 70);
    }
}