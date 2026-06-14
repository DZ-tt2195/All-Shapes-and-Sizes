using System.Collections.Generic;
using UnityEngine;

public class GameFiles : MonoBehaviour
{
    public static GameFiles inst;
    [SerializeField] List<Shape> mainShapes;
    [SerializeField] List<Shape> bonusShapes;
    Dictionary<string, Shape> shapeDictionary = new Dictionary<string, Shape>();
    void Awake()
    {
        inst = this;
        foreach (Shape shape in mainShapes)
            shapeDictionary.Add(shape.name, shape);
        foreach (Shape shape in bonusShapes)
            shapeDictionary.Add(shape.name, shape);
    }
    public List<Shape> AllMains() => mainShapes;
    public List<Shape> AllBonuses() => bonusShapes;
    public Shape GetShape(string shapeName) => shapeDictionary[shapeName]; 
}
