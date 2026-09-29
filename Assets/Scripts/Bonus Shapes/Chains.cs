using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using MyBox;

public class Chains : Shape
{
    int currentCount;
    [SerializeField] List<SpriteRenderer> miniCircles = new();
    [SerializeField] int arrowCount;
    public override string MyText() => AutoTranslate.Chains(miniCircles.Count.ToString(), arrowCount.ToString());
    public override void Setup(Vector2 start, bool cursed)
    {
        base.Setup(start, cursed);
        currentCount = 0;
        UpdateCircles();
    }
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(110, 50) : new(65, 30);
    }
    public override bool TrackNewShapes() => true;
    public override void OnNewShape(Shape newShape, CreationType creationType)
    {
        if (creationType == CreationType.Combine)
        {
            currentCount++;
            if (currentCount >= miniCircles.Count && this.HasAbility())
            {
                ShapeManager.inst.ReturnShape(this, ReturnType.Done);
                List<Type> toDrop = Enumerable.Repeat(typeof(Arrow), arrowCount).ToList();
                ShapeManager.inst.StartCoroutine(ShapeManager.inst.DropRandomly(toDrop, this.transform.position, 0.1f, 0.1f, true));
                return;
            }
        }
        else if (creationType == CreationType.Drop)
        {
            currentCount = 0;
        }
        UpdateCircles();
    }
    void UpdateCircles()
    {
        for (int i = 0; i<currentCount; i++)
            miniCircles[i].gameObject.SetActive(true);
        for (int i = currentCount; i<miniCircles.Count; i++)
            miniCircles[i].gameObject.SetActive(false);
    }
}