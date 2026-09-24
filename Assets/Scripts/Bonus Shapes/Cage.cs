using UnityEngine;
using TMPro;

public class Cage : Shape
{
    [SerializeField] int requirement;
    void Update()
    {
        this.textBox.text = $"{ShapeManager.inst.StreakCombines}";
        if (HasAbility())
        {
            if (ShapeManager.inst.StreakCombines >= requirement)
            {
                ShapeManager.inst.GenerateShape(typeof(Circle).Name, this.transform.position, CreationType.Drop);
                ShapeManager.inst.GenerateShape(typeof(Square).Name, this.transform.position, CreationType.Drop);
                ShapeManager.inst.GenerateShape(typeof(Arrow).Name, this.transform.position, CreationType.Drop);
                ShapeManager.inst.ReturnShape(this);
            }
        }
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 90) : new(50, 60);
    }
}
