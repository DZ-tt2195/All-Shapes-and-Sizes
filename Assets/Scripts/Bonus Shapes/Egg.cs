using UnityEngine;
using TMPro;

public class Egg : Shape
{
    int disappearOn;
    int currentScore;
    [SerializeField] int increment;
    [SerializeField] AudioClip breakSound;
    public override void Setup(Vector2 start)
    {
        disappearOn = ShapeManager.instance.dropped + increment;
        currentScore = ShapeManager.instance.score;
        base.Setup(start);
    }
    void Update()
    {
        int currentCount = disappearOn - ShapeManager.instance.dropped;
        this.textBox.text = $"{currentCount}";
        if (this.canInteract)
        {
            if (ShapeManager.instance.score > currentScore)
            {
                AudioManager.instance.PlaySound(breakSound, 0.3f);
                ShapeManager.instance.ReturnShape(this);
            }
            else if (currentCount == 0)
            {
                ShapeManager.instance.GenerateShape(typeof(Star).Name, this.transform.position, CreationType.Drop);
                ShapeManager.instance.ReturnShape(this);
            }
        }
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(65, 90) : new(50, 70);
    }
}
