using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnButton : MonoBehaviour, IPointerClickHandler
{
    int columnNumber;
    
    public void Setup(int column)
    {
        this.columnNumber = column;
        this.gameObject.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Spawn();

        else if (eventData.button == PointerEventData.InputButton.Right)
            Spawn();
    }
    void Spawn()
    {
        ShapeManager.inst.DropNewShape(this.transform.position, columnNumber);
    }
}