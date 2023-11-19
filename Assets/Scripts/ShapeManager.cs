using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using UnityEngine.UI;
using TMPro;

public class ShapeManager : MonoBehaviour
{
    public static ShapeManager instance;

    TMP_Text dataText;

    Image nextImage;
    Shape nextShape;

    int score = 0;
    int dropped = 0;
    bool canPlay = false;

    public List<Shape> listOfShapes = new List<Shape>();
    public List<Shape> droppedShapes = new List<Shape>();

    [ReadOnly] public Camera mainCam;
    [SerializeField] GameObject gameOverTransform;
    [ReadOnly] public Transform deathLine;
    Transform leftWall;
    Transform rightWall;

    private void Awake()
    {
        nextImage = GameObject.Find("Next Image").GetComponent<Image>();
        gameOverTransform.SetActive(false);
        instance = this;
        dataText = GameObject.Find("Data Text").GetComponent<TMP_Text>();
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
        StartCoroutine(DropRandomly());
    }

    IEnumerator DropRandomly()
    {
        dataText.transform.parent.gameObject.SetActive(false);
        nextImage.transform.parent.gameObject.SetActive(false);

        for (int i = 0; i < 75; i++)
        {
            yield return GenerateShape(listOfShapes[0], new Vector2(UnityEngine.Random.Range(leftWall.position.x + 0.6f, rightWall.position.x - 0.6f), deathLine.position.y - 0.15f));
        }

        yield return new WaitForSeconds(3f);
        canPlay = true;

        dataText.transform.parent.gameObject.SetActive(true);
        score = 0;
        dropped = 0;
        AddScore(0);
        RollNextShape();
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
            if (xValue < leftWall.position.x + 0.6f)
                xValue = leftWall.position.x + 0.6f;
            else if (xValue > rightWall.position.x - 0.6f)
                xValue = rightWall.position.x - 0.6f;

            dropped++;
            dataText.text = $"Score: {score} \nDropped: {dropped}";
            StartCoroutine(GenerateShape(nextShape, new Vector2(xValue, deathLine.position.y - 0.15f)));
            RollNextShape();
        }
    }

    void RollNextShape()
    {
        if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
        {
            nextShape = listOfShapes[0];
        }
        else
        {
            nextShape = droppedShapes[UnityEngine.Random.Range(0, droppedShapes.Count)];
        }

        nextImage.transform.parent.gameObject.SetActive(true);
        nextImage.sprite = nextShape.spriterenderer.sprite;
        nextImage.color = nextShape.spriterenderer.color;

        switch (nextShape.name)
        {
            case "Circle":
                nextImage.rectTransform.sizeDelta = new Vector2(50, 50);
                break;
            case "Square":
                nextImage.rectTransform.sizeDelta = new Vector2(65, 65);
                break;
            case "Rounded Square":
                nextImage.rectTransform.sizeDelta = new Vector2(90, 90);
                break;
            case "Bomb":
                nextImage.rectTransform.sizeDelta = new Vector2(50, 90);
                break;
        }

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

    public void Victory()
    {
        InputManager.instance.enabled = false;
        gameOverTransform.SetActive(true);
        gameOverTransform.transform.GetChild(0).GetComponent<TMP_Text>().text = "You won!";
    }

    public void GameOver()
    {
        InputManager.instance.enabled = false;
        gameOverTransform.SetActive(true);
        gameOverTransform.transform.GetChild(0).GetComponent<TMP_Text>().text = "You lost.";
    }
}
