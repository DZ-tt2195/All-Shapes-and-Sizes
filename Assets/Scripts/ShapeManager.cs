using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using TMPro;

public class ShapeManager : MonoBehaviour
{
    public static ShapeManager instance;

    TMP_Text dataText;
    TMP_Text nextText;
    Shape nextShape;

    int score = 0;
    int dropped = 0;
    bool canPlay = false;

    public List<Shape> listOfShapes = new List<Shape>();
    public List<Shape> otherShapes = new List<Shape>();

    [ReadOnly] public Camera mainCam;
    [SerializeField] GameObject gameOverTransform;
    Transform deathLine;
    Transform leftWall;
    Transform rightWall;

    private void Awake()
    {
        gameOverTransform.SetActive(false);
        instance = this;
        dataText = GameObject.Find("Data Text").GetComponent<TMP_Text>();
        nextText = GameObject.Find("Next Shape Text").GetComponent<TMP_Text>();
        mainCam = Camera.main;
        deathLine = GameObject.Find("Death Line").transform;
        leftWall = GameObject.Find("Left Wall").transform;
        rightWall = GameObject.Find("Right Wall").transform;
    }

    private void OnEnable()
    {
        Debug.Log("subscribed");
        InputManager.instance.OnStartTouch += DropShape;
    }

    private void OnDisable()
    {
        Debug.Log("unsubscribed");
        InputManager.instance.OnStartTouch -= DropShape;
    }

    private void Start()
    {
        RollNextShape();
        StartCoroutine(DropRandomly());
    }

    IEnumerator DropRandomly()
    {
        dataText.transform.parent.gameObject.SetActive(false);
        for (int i = 0; i < 75; i++)
        {
            yield return GenerateShape(listOfShapes[0], new Vector2(UnityEngine.Random.Range(leftWall.position.x + 0.5f, rightWall.position.x - 0.5f), deathLine.position.y - 0.1f));
        }

        yield return new WaitForSeconds(2.5f);
        canPlay = true;

        dataText.transform.parent.gameObject.SetActive(true);
        score = 0;
        dropped = 0;
        AddScore(0);
    }

    Vector2 GetWorldCoordinates(Vector2 screenPos)
    {
        Vector2 screenCoord = new Vector2(screenPos.x, screenPos.y);
        Vector2 worldCoord = mainCam.ScreenToWorldPoint(screenCoord);
        return worldCoord;
    }

    void DropShape(Vector2 screenPosition)
    {
        if (canPlay)
        {
            float xValue = GetWorldCoordinates(screenPosition).x;
            if (xValue < leftWall.position.x + 0.5f)
                xValue = leftWall.position.x + 0.5f;
            else if (xValue > rightWall.position.x - 0.5f)
                xValue = rightWall.position.x - 0.5f;

            dropped++;
            dataText.text = $"Score: {score} \nDropped: {dropped}";
            StartCoroutine(GenerateShape(nextShape, new Vector2(xValue, deathLine.position.y - 0.1f)));
            RollNextShape();
        }
    }

    void RollNextShape()
    {
        if (UnityEngine.Random.Range(0f, 1f) < 0.8f)
        {
            nextShape = listOfShapes[0];
        }
        else
        {
            nextShape = otherShapes[UnityEngine.Random.Range(0, otherShapes.Count)];
        }

        nextText.text = $"Next: \n{nextShape.name}";
    }

    public IEnumerator GenerateShape(Shape shape, Vector2 spawn)
    {
        yield return new WaitForSeconds(0.05f);

        try
        {
            Shape newShape = Instantiate(shape);
            newShape.transform.position = spawn;

            if (listOfShapes.Contains(shape))
            {
                newShape.Setup(listOfShapes.IndexOf(shape));
            }
            else if (newShape.name == "Bomb(Clone)")
            {
                newShape.Bomb();
            }
        }
        catch (ArgumentOutOfRangeException)
        {
            //do nothing
        }
    }

    public void AddScore(int num)
    {
        if (canPlay)
        {
            score += num;
            dataText.text = $"Score: {score} \nDropped: {dropped}";
        }
    }

    public void GameOver()
    {
        InputManager.instance.enabled = false;
        gameOverTransform.SetActive(true);
    }
}
