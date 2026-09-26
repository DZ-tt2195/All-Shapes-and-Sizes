using UnityEngine;

public class Flashlight : Shape
{
    [SerializeField] int requirement;
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(110, 50) : new(65, 25);
    }
    void Update()
    {
        textBox.text = $"{shapesTouchingThis.Count}";
        if (shapesTouchingThis.Count >= requirement)
        {
            ShapeManager.inst.ReturnShape(this);
            ShapeManager.inst.GenerateShape(typeof(Star).Name, this.transform.position, CreationType.Other);
        }
    }
}