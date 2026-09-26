using UnityEngine;

public class GhostStar : Star
{
    int disappearOn;
    [SerializeField] int starting;
    [SerializeField] AudioClip vanishSound;
    public override void Setup(Vector2 start, bool cursed)
    {
        base.Setup(start, cursed);
        disappearOn = starting;
        this.textBox.text = $"{disappearOn}";
    }
    public override bool TrackNewShapes() => true;
    public override void OnNewShape(Shape newShape, CreationType creationType)
    {
        if (creationType == CreationType.Drop)
        {
            disappearOn--;
            this.textBox.text = $"{disappearOn}";
            if (disappearOn == 0)
            {
                AudioManager.instance.PlaySound(vanishSound, 0.3f);
                ShapeManager.inst.ReturnShape(this);    
            }
        }
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(100, 100) : new(60, 60);
    }
}