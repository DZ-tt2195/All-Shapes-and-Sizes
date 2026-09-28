using UnityEngine;
using TMPro;

public class New2 : Shape
{
    int disappearOn;
    [SerializeField] int starting;
    public override string MyText() => AutoTranslate.Egg(starting.ToString());
    public override void Setup(Vector2 start, bool cursed)
    {
        base.Setup(start, cursed);
        disappearOn = starting;
        textBox.text = $"{disappearOn}";
    }
    public override bool TrackNewShapes() => true;
    public override void OnNewShape(Shape newShape, CreationType creationType)
    {
        if (creationType == CreationType.Drop)
        {
            disappearOn = Mathf.Max(0, disappearOn-1);
            this.textBox.text = $"{disappearOn}";
            if (disappearOn == 0 && this.HasAbility())
            {
                Vector2 thisSpawn = this.transform.position;
                ShapeManager.inst.ReturnShape(this);
                ShapeManager.inst.ReturnShape(newShape);
                ShapeManager.inst.GenerateShape(newShape.GetType().Name, thisSpawn, CreationType.Drop);
            }
        }
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(65, 90) : new(50, 70);
    }
}