using UnityEngine;

public class Skull : Shape
{
    int current;
    [SerializeField] int starting;
    public override string MyText() => AutoTranslate.Skull(starting.ToString());
    protected override void Setup()
    {
        current = starting;
        this.textBox.text = $"{current}";
        EventManager.inst.Subscribe<CreatedShape>(OnNewShape);
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 90) : new(60, 60);
    }
    void OnNewShape(CreatedShape info)
    {
        if (info.newShape != this && info.type == CreationType.Combine && info.newShape.IsMainShape() && !info.newShape.cursed)
        {
            AudioManager.instance.Curse();
            info.newShape.CursedStatus(true);
            current--;
            this.textBox.text = $"{current}";
            if (current == 0)
                ShapeManager.inst.ReturnShape(this, ReturnType.Done);
        }        
    }
    public override void OnReturn(ReturnType returnType)
    {
        EventManager.inst.Unsubscribe<CreatedShape>(OnNewShape);
    }
}