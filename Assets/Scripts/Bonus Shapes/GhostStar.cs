using UnityEngine;

public class GhostStar : Star
{
    int disappearOn;
    [SerializeField] int starting;
    [SerializeField] AudioClip vanishSound;
    public override string MyText() => AutoTranslate.GhostStar(starting.ToString());
    public override void Setup(Vector2 start, bool cursed)
    {
        base.Setup(start, cursed);
        disappearOn = starting;
        this.textBox.text = $"{disappearOn}";
        EventManager.inst.Subscribe<CreatedShape>(OnNewShape);
    }
    void OnNewShape(CreatedShape info)
    {
        if (info.newShape != this && info.type == CreationType.Drop)
        {
            disappearOn--;
            this.textBox.text = $"{disappearOn}";
            if (disappearOn == 0)
            {
                AudioManager.instance.PlaySound(vanishSound, 0.3f);
                ShapeManager.inst.ReturnShape(this, ReturnType.Destroy);    
            }
        }
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(100, 100) : new(60, 60);
    }
    public override void OnReturn(ReturnType returnType)
    {
        EventManager.inst.Unsubscribe<CreatedShape>(OnNewShape);
    }
}