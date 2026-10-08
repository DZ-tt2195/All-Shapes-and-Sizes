using UnityEngine;
using TMPro;

public class Egg : Shape
{
    int disappearOn;
    [SerializeField] int starting;
    [SerializeField] AudioClip breakSound;
    public override string MyText() => AutoTranslate.Egg(starting.ToString());
    public override void Setup(Vector2 start, bool cursed)
    {
        base.Setup(start, cursed);
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
            if (disappearOn == 0 && this.HasAbility())
            {
                ShapeManager.inst.ReturnShape(this, ReturnType.Done);
                ShapeManager.inst.GenerateShape(typeof(Star).Name, this.transform.position, CreationType.Special);
            }
        }
        else if (info.type == CreationType.Combine)
        {
            AudioManager.instance.PlaySound(breakSound, 0.3f);
            ShapeManager.inst.ReturnShape(this, ReturnType.Destroy);            
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