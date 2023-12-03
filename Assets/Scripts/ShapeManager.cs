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

    Image nextImage1;
    Shape nextShape1;
    Image nextImage2;
    Shape nextShape2;

    int score = 0;
    int dropped = 0;
    [SerializeField] int startingDrop;
    [SerializeField] int dropLimit;

    public List<Shape> listOfShapes = new();
    public List<ChanceOfDrop> droppedShapes = new();
    List<Shape> toDrop = new();

    [ReadOnly] public Camera mainCam;
    [SerializeField] GameObject gameOverTransform;

    float currentGravity = 2.5f;
    Button resign;
    bool hasEnded = false;

    [ReadOnly] public Transform deathLine;
    Transform floor;
    Transform ceiling;
    Transform leftWall;
    Transform rightWall;
    [SerializeField] PointsVisual pv;

#region Setup

    private void Awake()
    {
        resign = GameObject.Find("Resign").GetComponent<Button>();
        nextImage1 = GameObject.Find("Next Image 1").GetComponent<Image>();
        nextImage2 = GameObject.Find("Next Image 2").GetComponent<Image>();
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
        InputManager.instance.enabled = false;
        resign.onClick.AddListener(() => GameOver("You're out of shape(s).", false));
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

        nextShape1 = toDrop[UnityEngine.Random.Range(0, toDrop.Count)];
        nextImage1.sprite = nextShape1.spriterenderer.sprite;
        nextImage1.color = nextShape1.spriterenderer.color;

        nextShape2 = toDrop[UnityEngine.Random.Range(0, toDrop.Count)];
        nextImage2.sprite = nextShape2.spriterenderer.sprite;
        nextImage2.color = nextShape2.spriterenderer.color;
    }

    IEnumerator DropRandomly()
    {
        dataText.transform.parent.gameObject.SetActive(false);
        nextImage1.transform.parent.gameObject.SetActive(false);
        nextImage2.transform.parent.gameObject.SetActive(false);

        for (int i = 0; i < startingDrop; i++)
        {
            yield return GenerateShape(listOfShapes[0], new Vector2(UnityEngine.Random.Range(leftWall.position.x + 0.6f, rightWall.position.x - 0.6f), deathLine.position.y - 0.15f));
        }

        yield return new WaitForSeconds(3f);

        dataText.transform.parent.gameObject.SetActive(true);
        InputManager.instance.enabled = true;
        score = 0;
        dropped = 0;
        AddScore(0, null);
        RollNextShape();
    }

#endregion

#region Dropping Shapes

    Vector2 GetWorldCoordinates(Vector2 screenPos)
    {
        Vector2 screenCoord = new(screenPos.x, screenPos.y);
        Vector2 worldCoord = mainCam.ScreenToWorldPoint(screenCoord);
        return worldCoord;
    }

    void DropShape(Vector2 screenPosition)
    {
        float yValue = (currentGravity > 0) ? deathLine.position.y - 0.6f : deathLine.position.y + 0.6f;
        float xValue = GetWorldCoordinates(screenPosition).x;

        if (xValue > (leftWall.position.x + 0.3f) && xValue < (rightWall.position.x - 0.3f))
        {
            if (nextShape1.textBox != null)
                dropped++;

            UpdateDataText();
            StartCoroutine(GenerateShape(nextShape1, new Vector2(xValue, yValue)));
            RollNextShape();
            StartCoroutine(OutOfShapes());
        }

    }

    IEnumerator OutOfShapes()
    {
        if (LevelSettings.instance.setting != TitleScreen.Setting.Endless && dropped >= dropLimit)
        {
            nextImage1.transform.parent.gameObject.SetActive(false);
            nextImage2.transform.parent.gameObject.SetActive(false);
            dataText.transform.parent.gameObject.SetActive(false);
            InputManager.instance.enabled = false;
            yield return new WaitForSeconds(2.5f);
            GameOver("You're Out Of Shapes.", false);
        }
    }

    void RollNextShape()
    {
        nextShape1 = nextShape2;
        nextImage1.transform.parent.gameObject.SetActive(true);
        nextImage1.sprite = nextShape2.spriterenderer.sprite;
        nextImage1.color = nextShape2.spriterenderer.color;

        do{nextShape2 = toDrop[UnityEngine.Random.Range(0, toDrop.Count)];
        } while (nextShape1.name == "Inversion" && nextShape2.name == "Inversion");

        nextImage2.transform.parent.gameObject.SetActive(true);
        nextImage2.sprite = nextShape2.spriterenderer.sprite;
        nextImage2.color = nextShape2.spriterenderer.color;

        switch (nextShape1.name)
        {
            case "Circle":
                nextImage1.rectTransform.sizeDelta = new Vector2(50, 50);
                break;
            case "Square":
                nextImage1.rectTransform.sizeDelta = new Vector2(65, 65);
                break;
            case "Arrow":
                nextImage1.rectTransform.sizeDelta = new Vector2(100, 100);
                break;
            case "Diamond":
                nextImage1.rectTransform.sizeDelta = new Vector2(110, 60);
                break;
            case "Bomb":
                nextImage1.rectTransform.sizeDelta = new Vector2(50, 90);
                break;
            case "Inversion":
                nextImage1.rectTransform.sizeDelta = new Vector2(90, 90);
                break;
        }

        switch (nextShape2.name)
        {
            case "Circle":
                nextImage2.rectTransform.sizeDelta = new Vector2(40, 40);
                break;
            case "Square":
                nextImage2.rectTransform.sizeDelta = new Vector2(50, 50);
                break;
            case "Arrow":
                nextImage2.rectTransform.sizeDelta = new Vector2(70, 70);
                break;
            case "Diamond":
                nextImage2.rectTransform.sizeDelta = new Vector2(80, 50);
                break;
            case "Bomb":
                nextImage2.rectTransform.sizeDelta = new Vector2(40, 80);
                break;
            case "Inversion":
                nextImage2.rectTransform.sizeDelta = new Vector2(60, 60);
                break;
        }
    }

    public IEnumerator GenerateShape(Shape shape, Vector2 spawn)
    {
        yield return new WaitForSeconds(0.05f);
        Shape newShape = Instantiate(shape);
        newShape.transform.position = spawn;

        if (shape.textBox != null)
        {
            newShape.Setup(listOfShapes.IndexOf(shape), currentGravity);
        }
        else
        {
            newShape.AltShape(currentGravity);
        }
    }

#endregion

#region Other

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

    public void SwitchGravity()
    {
        if (InputManager.instance.enabled)
        {
            InputManager.instance.enabled = false;
            nextImage1.transform.parent.gameObject.SetActive(false);
            nextImage2.transform.parent.gameObject.SetActive(false);

            floor.gameObject.SetActive(true);
            ceiling.gameObject.SetActive(true);
            deathLine.gameObject.SetActive(false);

            currentGravity *= -1;
            Shape[] allShapes = FindObjectsOfType<Shape>();
            foreach (Shape shape in allShapes)
                shape.rb.gravityScale = currentGravity;

            StartCoroutine(UnPauseGame());
        }
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

        InputManager.instance.enabled = true;
        deathLine.gameObject.SetActive(true);
        nextImage1.transform.parent.gameObject.SetActive(true);
        nextImage2.transform.parent.gameObject.SetActive(true);
    }

    public void AddScore(int score, Shape shape)
    {
        if (dataText.transform.parent.gameObject.activeSelf)
        {
            this.score += score;
            if (shape != null)
            {
                PointsVisual newPV = Instantiate(pv);
                newPV.Setup(score, shape);
            }
            UpdateDataText();
        }
    }

    public void GameOver(string message, bool won)
    {
        if (!hasEnded)
        {
            InputManager.instance.enabled = false;
            gameOverTransform.SetActive(true);
            gameOverTransform.transform.GetChild(0).GetComponent<TMP_Text>().text = message;
            hasEnded = true;

            if (LevelSettings.instance.setting == TitleScreen.Setting.Endless && PlayerPrefs.GetInt($"{SceneManager.GetActiveScene().name} - Endless") < score)
                PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name} - Endless", score);
            else if (won && LevelSettings.instance.setting == TitleScreen.Setting.MergeCrown)
                PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name} - Merge", 1);
            else if (won && LevelSettings.instance.setting == TitleScreen.Setting.ReachScore)
                PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name} - Score", 1);
        }
    }

#endregion

}
