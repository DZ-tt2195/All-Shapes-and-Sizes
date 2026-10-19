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
            var (left, right) = ShapeManager.inst.XSpawnRange();
            float gap = 0.5f;
            for (float i = left+gap; i<=right-gap; i+=(gap*2))
            {
                ShapeManager.inst.GenerateShape(typeof(Circle).Name, new Vector2(i, this.transform.position.y), CreationType.Special);
            }
        }
    }
}