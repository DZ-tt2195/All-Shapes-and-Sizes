using System.Collections.Generic;
using UnityEngine;

public class Snowflake : Shape
{
    int disappearOn;
    [SerializeField] int starting;
    [SerializeField] AudioClip dingSound;
    public override string MyText() => AutoTranslate.Snowflake(starting.ToString());
    public override void Setup(Vector2 start, bool cursed)
    {
        base.Setup(start, cursed);
        disappearOn = starting;
        this.textBox.text = $"{disappearOn}";
        EventManager.inst.Subscribe<CreatedShape>(OnNewShape);
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 90) : new(60, 60);
    }
    void OnNewShape(CreatedShape info)
    {
        if (info.newShape != this && info.type == CreationType.Combine)
        {
            disappearOn--;
            this.textBox.text = $"{disappearOn}";
            if (disappearOn == 0)
            {
                ShapeManager.inst.ReturnShape(this, ReturnType.Done);
                AudioManager.instance.PlaySound(dingSound, 0.3f);                
            }
        }        
    }
    public override void OnReturn(ReturnType returnType)
    {
        EventManager.inst.Unsubscribe<CreatedShape>(OnNewShape);
    }
}