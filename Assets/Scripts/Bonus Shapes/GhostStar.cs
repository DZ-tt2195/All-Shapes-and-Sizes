using UnityEngine;

public class GhostStar : Star
{
    int disappearOn;
    [SerializeField] int increment;
    [SerializeField] AudioClip vanishSound;
    public override void Setup(Vector2 start, bool cursed)
    {
        disappearOn = ShapeManager.instance.DropCount + increment;
        base.Setup(start, cursed);
    }
    void Update()
    {
        int currentCount = disappearOn-ShapeManager.instance.DropCount;
        this.textBox.text = $"{currentCount}";
        if (HasAbility() && currentCount == 0)
        {
            AudioManager.instance.PlaySound(vanishSound, 0.3f);
            ShapeManager.instance.ReturnShape(this);
        }
    }
}
