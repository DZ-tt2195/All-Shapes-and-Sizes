using UnityEngine;

public class GhostStar : Star
{
    int disappearOn;
    [SerializeField] int increment;
    public override void Setup(Vector2 start)
    {
        disappearOn = ShapeManager.instance.dropped + increment;
        base.Setup(start);
    }
    void Update()
    {
        int currentCount = disappearOn-ShapeManager.instance.dropped;
        this.textBox.text = $"{currentCount}";
        if (this.canInteract && currentCount == 0)
            ShapeManager.instance.ReturnShape(this);
    }
}
