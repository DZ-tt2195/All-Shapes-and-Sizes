using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ChanceOfDrop
{
    public Shape shape;
    public int chance;
}

public class ShapeManager : MonoBehaviour
{
    public static ShapeManager instance;

    TMP_Text dataText;

    Image nextImage;
    Shape nextShape;

    int score = 0;
    int dropped = 0;
    [SerializeField] int startingDrop;
    [SerializeField] int dropLimit;
    [ReadOnly] public bool canPlay = false;

    public List<Shape> listOfShapes = new List<Shape>();
    public List<ChanceOfDrop> droppedShapes = new List<ChanceOfDrop>();
    List<Shape> toDrop = new List<Shape>();

    [ReadOnly] public Camera mainCam;
    [SerializeField] GameObject gameOverTransform;

    int currentGravity = 2;

    [ReadOnly] public Transform deathLine;
    Transform floor;
    Transform ceiling;
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
        floor = GameObject.Find("Floor").transform;
        ceiling = GameObject.Find("Ceiling").transform;
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
        ceiling.gameObject.SetActive(false);
        deathLine.transform.localPosition = new Vector3(0, ceiling.transform.localPosition.y + 0.15f, 0);

        StartCoroutine(DropRandomly());
        foreach(ChanceOfDrop next in droppedShapes)
        {
            for (int i = 0; i < next.chance; i++)
                toDrop.Add(next.shape);
        }
    }

    IEnumerator DropRandomly()
    {
        dataText.transform.parent.gameObject.SetActive(false);
        nextImage.transform.parent.gameObject.SetActive(false);

        for (int i = 0; i < startingDrop; i++)
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
            float yValue = (currentGravity > 0) ? deathLine.position.y - 0.15f : deathLine.position.y + 0.15f;
            float xValue = GetWorldCoordinates(screenPosition).x;
            if (xValue < leftWall.position.x + 0.6f)
                xValue = leftWall.position.x + 0.6f;
            else if (xValue > rightWall.position.x - 0.6f)
                xValue = rightWall.position.x - 0.6f;

            dropped++;
            dataText.text = $"Score: {score} \nDropped: {dropped}/{dropLimit}";
            StartCoroutine(GenerateShape(nextShape, new Vector2(xValue, yValue)));
            RollNextShape();
            StartCoroutine(OutOfShapes());
        }
    }

    IEnumerator OutOfShapes()
    {
        if (dropped >= dropLimit)
        {
            dataText.transform.parent.gameObject.SetActive(false);
            InputManager.instance.enabled = false;
            yield return new WaitForSeconds(2.5f);
            GameOver("You're Out Of Shape(s).");
        }
    }

    void RollNextShape()
    {
        nextShape = toDrop[UnityEngine.Random.Range(0, toDrop.Count)];
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
            case "Arrow":
                nextImage.rectTransform.sizeDelta = new Vector2(100, 100);
                break;
            case "Diamond":
                nextImage.rectTransform.sizeDelta = new Vector2(120, 60);
                break;
            case "Bomb":
                nextImage.rectTransform.sizeDelta = new Vector2(50, 90);
                break;
            case "Inversion":
                nextImage.rectTransform.sizeDelta = new Vector2(80, 80);
                break;
        }

    }

    public IEnumerator GenerateShape(Shape shape, Vector2 spawn)
    {
        yield return new WaitForSeconds(0.05f);
        Shape newShape = Instantiate(shape);
        newShape.transform.position = spawn;

        if (listOfShapes.Contains(shape))
        {
            newShape.Setup(listOfShapes.IndexOf(shape), currentGravity);
        }
        else
        {
            newShape.AltShape(currentGravity);
        }
    }

    public void SwitchGravity()
    {
        canPlay = false;
        nextImage.transform.parent.gameObject.SetActive(false);
        floor.gameObject.SetActive(true);
        ceiling.gameObject.SetActive(true);
        deathLine.gameObject.SetActive(false);

        currentGravity *= -1;
        Shape[] allShapes = FindObjectsOfType<Shape>();
        foreach (Shape shape in allShapes)
            shape.rb.gravityScale = currentGravity;

        StartCoroutine(UnPauseGame());
    }

    IEnumerator UnPauseGame()
    {
        yield return new WaitForSeconds(1.5f);
        if (currentGravity < 0)
        {
            deathLine.transform.localEulerAngles = new Vector3(0, 0, -180);
            deathLine.transform.localPosition = new Vector3(0, floor.transform.localPosition.y - 0.15f, 0);
            floor.gameObject.SetActive(false);
        }
        else
        {
            deathLine.transform.localEulerAngles = new Vector3(0, 0, 0);
            deathLine.transform.localPosition = new Vector3(0, ceiling.transform.localPosition.y + 0.15f, 0);
            ceiling.gameObject.SetActive(false);
        }

        deathLine.gameObject.SetActive(true);
        canPlay = true;
        nextImage.transform.parent.gameObject.SetActive(true);
    }

    public void AddScore(int num)
    {
        if (canPlay)
        {
            score += num;
            dataText.text = $"Score: {score} \nDropped: {dropped}/{dropLimit}";
        }
    }

    public void GameOver(string message)
    {
        if (canPlay)
        {
            InputManager.instance.enabled = false;
            gameOverTransform.SetActive(true);
            gameOverTransform.transform.GetChild(0).GetComponent<TMP_Text>().text = message;
            canPlay = false;
        }
    }
}
