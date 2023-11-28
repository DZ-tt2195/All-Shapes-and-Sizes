using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    TMP_Text tutorialText;

    Image nextImage;
    Shape nextShape;

    int score = 0;
    int dropped = 0;
    [SerializeField] int startingDrop;
    [SerializeField] int dropLimit;
    [ReadOnly] public bool canPlay = false;

    public List<Shape> listOfShapes = new();
    public List<ChanceOfDrop> droppedShapes = new();
    List<Shape> toDrop = new();

    [ReadOnly] public Camera mainCam;
    [SerializeField] GameObject gameOverTransform;

    float currentGravity = 2.5f;

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
        tutorialText = GameObject.Find("Tutorial Text").GetComponent<TMP_Text>();
        mainCam = Camera.main;
        deathLine = GameObject.Find("Death Line").transform;
        leftWall = GameObject.Find("Left Wall").transform;
        rightWall = GameObject.Find("Right Wall").transform;
        floor = GameObject.Find("Floor").transform;
        ceiling = GameObject.Find("Ceiling").transform;
    }

    private void OnEnable()
    {
        InputManager.instance.OnStartTouch += DropShape;
    }

    private void OnDisable()
    {
        InputManager.instance.OnStartTouch -= DropShape;
    }

    private void Start()
    {
        ceiling.gameObject.SetActive(false);
        deathLine.transform.localPosition = new Vector3(0, ceiling.transform.localPosition.y + 0.15f, 0);

        tutorialText.text =
        "Touch the screen to drop shapes down the tube. When a shape touches another of the same shape, they merge into a larger one. ";

        switch (LevelSettings.instance.setting)
        {
            case TitleScreen.Setting.MergeCrown:
                tutorialText.text += $"If you let any shapes go above the top, or drop more than {dropLimit} shapes you lose." +
                "\n\nTo win, create 2 Crowns, and then have them merge with one another.";
                break;
            case TitleScreen.Setting.ReachScore:
                dropLimit += 100;
                tutorialText.text += $"If you let any shapes go above the top, or drop more than {dropLimit} shapes you lose." +
                "\n\nTo win, get a score above 2000 by merging shapes together.";
                break;
            case TitleScreen.Setting.Endless:
                tutorialText.text += "If you let any shapes go above the top, you lose." +
                "\n\nPlay for as long as you are able to until you lose.";
                break;
        }


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
        Vector2 screenCoord = new(screenPos.x, screenPos.y);
        Vector2 worldCoord = mainCam.ScreenToWorldPoint(screenCoord);
        return worldCoord;
    }

    void UpdateDataText()
    {
        if (LevelSettings.instance.setting == TitleScreen.Setting.Endless)
        {
            char infinitySymbol = '\u221E';
            dataText.text = $"Score: {score} \nDropped: {dropped}/{infinitySymbol}";
        }
        else
        {
            dataText.text = $"Score: {score} \nDropped: {dropped}/{dropLimit}";

            if (LevelSettings.instance.setting == TitleScreen.Setting.ReachScore && score >= 2000)
                GameOver("You Won!", true);
        }
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

            if (listOfShapes.Contains(nextShape))
                dropped++;

            UpdateDataText();
            StartCoroutine(GenerateShape(nextShape, new Vector2(xValue, yValue)));
            RollNextShape();

	    if (LevelSettings.instance.setting != TitleScreen.Setting.Endless)
            StartCoroutine(OutOfShapes());
        }
    }

    IEnumerator OutOfShapes()
    {
        if (dropped >= dropLimit)
        {
            nextImage.transform.parent.gameObject.SetActive(false);
            dataText.transform.parent.gameObject.SetActive(false);
            InputManager.instance.enabled = false;
            yield return new WaitForSeconds(2.5f);
            GameOver("You're Out Of Shape(s).", false);
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
            UpdateDataText();
        }
    }

    public void GameOver(string message, bool won)
    {
        if (canPlay)
        {
            InputManager.instance.enabled = false;
            gameOverTransform.SetActive(true);
            gameOverTransform.transform.GetChild(0).GetComponent<TMP_Text>().text = message;
            canPlay = false;

            if (LevelSettings.instance.setting == TitleScreen.Setting.Endless && PlayerPrefs.GetInt($"{SceneManager.GetActiveScene().name} - Endless") < score)
                PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name} - Endless", score);
            else if (won && LevelSettings.instance.setting == TitleScreen.Setting.MergeCrown)
                PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name} - Merge", 1);
            else if (won && LevelSettings.instance.setting == TitleScreen.Setting.ReachScore)
                PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name} - Score", 1);
        }
    }
}
