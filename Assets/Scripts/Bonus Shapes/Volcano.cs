using System.Collections.Generic;
using UnityEngine;

public class Volcano : Shape
{
    int currentCount = 4;
    Shape target;
    [SerializeField] LineRenderer lineRenderer;
    protected override void Setup()
    {
        target = null;
        currentCount = 4;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.sortingOrder = 10;    
    }
    void Update()
    {
        if (target == null || !target.gameObject.activeSelf)
        {
            currentCount--;
            target = ShapeManager.inst.GenerateShape(typeof(Square).Name, this.transform.position, CreationType.Special);
            if (currentCount == 0)
                ShapeManager.inst.ReturnShape(this, ReturnType.Done);
        }
        lineRenderer.SetPosition(0, this.transform.position);
        lineRenderer.SetPosition(1, target.transform.position);
        this.textBox.text = currentCount.ToString();
    }
}