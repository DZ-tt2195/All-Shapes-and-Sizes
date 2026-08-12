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
    public HashSet<Shape> SavedBonusShapes()
    {
        HashSet<Shape> toReturn = new();
        for (int i = 0; i<NewCustomizer.numBonusShapes; i++)
        {
            int savedNumber = PrefManager.GetShape(i);
            if (savedNumber != -1)
                toReturn.Add(bonusShapes[savedNumber]);
        }
        while (toReturn.Count < NewCustomizer.numBonusShapes)
        {
            int randomNumber = Random.Range(0, bonusShapes.Count);
            toReturn.Add(bonusShapes[randomNumber]);                
        }
        return toReturn;
    }
}
