using UnityEngine;

public class Flashlight : Shape
{
    [SerializeField] int requirement;
    public override string MyText() => AutoTranslate.Flashlight(requirement.ToString());
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(110, 50) : new(65, 30);
    }
    void Update()
    {
        textBox.text = $"{shapesTouchingThis.Count}";
        if (shapesTouchingThis.Count >= requirement)
        {
            ShapeManager.inst.ReturnShape(this, ReturnType.Done);
            ShapeManager.inst.GenerateShape(typeof(Star).Name, this.transform.position, CreationType.Special);
        }
    }
}