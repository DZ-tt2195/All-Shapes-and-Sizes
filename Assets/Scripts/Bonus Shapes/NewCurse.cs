using UnityEngine;

public class NewCurse : Shape
{
    int current;
    [SerializeField] int starting;
    [SerializeField] AudioClip dingSound;
    [SerializeField] AudioClip poisonSound;
    public override string MyText() => AutoTranslate.NewCurse(starting.ToString());
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
            AudioManager.instance.PlaySound(poisonSound, 0.3f);
            info.newShape.CursedStatus(true);
            current--;
            this.textBox.text = $"{current}";
            if (current == 0)
            {
                AudioManager.instance.PlaySound(dingSound, 0.3f);
                ShapeManager.inst.ReturnShape(this, ReturnType.Done);
            }
        }        
    }
    public override void OnReturn(ReturnType returnType)
    {
        EventManager.inst.Unsubscribe<CreatedShape>(OnNewShape);
    }
}