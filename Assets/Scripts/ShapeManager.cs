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

#region Variables

    public static ShapeManager instance;
    [ReadOnly] public Camera mainCam;

    [Foldout("Audio", true)]
        public AudioClip scoreSound;
        [SerializeField] AudioClip dropSound;
        [SerializeField] AudioClip timerSound;
        [SerializeField] AudioClip gravitySound;
        public AudioClip bombSound;
        [SerializeField] AudioClip winSound;
        [SerializeField] AudioClip loseSound;

    [Foldout("Text", true)]
        [SerializeField] TMP_Text dataText;
        [SerializeField] TMP_Text tutorialText;
        [SerializeField] TMP_Text warningText;

    [Foldout("Next shapes", true)]
        [SerializeField] Image nextImage1;
        Shape nextShape1;
        [SerializeField] Image nextImage2;
        Shape nextShape2;

    [Foldout("To drop", true)]
        public List<Shape> listOfShapes = new();
        public List<ChanceOfDrop> droppedShapes = new();
        List<Shape> toDrop = new();
        [SerializeField] Transform gravityArrow;
        float currentGravity = 2.5f;

    [Foldout("Score", true)]
        int score = 0;
        int dropped = 0;
        [SerializeField] PointsVisual pv;
        [SerializeField] Button hideUI;

    [Foldout("Game end", true)]
        [SerializeField] GameObject gameOverTransform;
        [SerializeField] Button resign;
        bool hasEnded = false;

    [Foldout("Level geometry", true)]
        [ReadOnly] public Transform deathLine;
        Transform floor;
        Transform ceiling;
        Transform leftWall;
        Transform rightWall;

    #endregion

#region Setup

    private void Awake()
    {
        gameOverTransform.SetActive(false);
        instance = this;
        mainCam = Camera.main;
        deathLine = GameObject.Find("Death Line").transform;
        leftWall = GameObject.Find("Left Wall").transform;
        rightWall = GameObject.Find("Right Wall").transform;
        floor = GameObject.Find("Floor").transform;
        ceiling = GameObject.Find("Ceiling").transform;
    }

    private void OnEnable()
    {
        if (Application.isMobilePlatform)
        {
            InputManager.instance.OnStartTouch += DropShape;
            Debug.Log("playing on phone");
        }
        else
            Debug.Log("playing on computer");
    }

    private void OnDisable()
    {
        if (Application.isMobilePlatform)
            InputManager.instance.OnStartTouch -= DropShape;
    }

    private void Start()
    {
        warningText.transform.localScale = new Vector2(0, 0);
        InputManager.instance.enabled = false;
        gravityArrow.transform.localScale = new Vector2(0, 0);
        gravityArrow.transform.localEulerAngles = new Vector3(0, 0, -90);
        resign.onClick.AddListener(() => GameOver("You gave up.", false));
        hideUI.onClick.AddListener(ToggleUI);
        ceiling.gameObject.SetActive(false);
        deathLine.transform.localPosition = new Vector3(0, ceiling.transform.localPosition.y + 0.15f, 0);

        if (Application.isMobilePlatform)
        {
            tutorialText.text =
            "Touch the screen to drop shapes down the tube. When a shape touches another of the same shape, they merge into a larger one. ";
        }
        else
        {
            tutorialText.text =
            "Click on the screen to drop shapes down the tube. When a shape touches another of the same shape, they merge into a larger one.\n\n";
        }

        switch (LevelSettings.instance.setting)
        {
            case TitleScreen.Setting.MergeCrown:
                tutorialText.text += $"If you let any shapes go above the top, or drop more than 150 shapes, you lose." +
                "\n\nTo win, create 2 Crowns, and then have them merge with one another.";
                break;
            case TitleScreen.Setting.Drops:
                tutorialText.text += $"If you let any shapes go above the top, or go above 500 points, you lose." +
                "\n\nTo win, drop 75 shapes.";
                break;
            case TitleScreen.Setting.MaxDrop:
                tutorialText.text += "If you let any shapes go above the top, or go above 1500 points, you lose." +
                "\n\nPlay for as long as you are able to until you lose.";
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

        int randRemoval = UnityEngine.Random.Range(0, toDrop.Count);
        nextShape1 = toDrop[randRemoval];
        toDrop.RemoveAt(randRemoval);
        nextImage1.sprite = nextShape1.spriterenderer.sprite;
        nextImage1.color = nextShape1.spriterenderer.color;

        randRemoval = UnityEngine.Random.Range(0, toDrop.Count);
        nextShape2 = toDrop[randRemoval];
        toDrop.RemoveAt(randRemoval);
        nextImage2.sprite = nextShape2.spriterenderer.sprite;
        nextImage2.color = nextShape2.spriterenderer.color;
    }

    IEnumerator DropRandomly()
    {
        dataText.transform.parent.gameObject.SetActive(false);
        nextImage1.transform.parent.gameObject.SetActive(false);
        nextImage2.transform.parent.gameObject.SetActive(false);

        for (int i = 0; i < 75; i++)
        {
            AudioManager.instance.PlaySound(dropSound, 0.2f);
            yield return GenerateShape(listOfShapes[0], new Vector2(UnityEngine.Random.Range(leftWall.position.x + 0.6f, rightWall.position.x - 0.6f), deathLine.position.y - 0.15f));
        }

        yield return new WaitForSeconds(2f);

        PointsVisual newPV = Instantiate(pv);
        newPV.Setup("Begin!", new Vector3(0, 0, -1), 1f);
        AudioManager.instance.PlaySound(winSound, 0.5f);

        yield return new WaitForSeconds(1f);

        dataText.transform.parent.gameObject.SetActive(true);
        InputManager.instance.enabled = true;
        score = 0;
        dropped = 0;
        AddScore(0, null);
        RollNextShape();
    }

#endregion

#region Dropping Shapes

    private void Update()
    {
        if (InputManager.instance.enabled && !Application.isMobilePlatform)
        {
            if (Input.GetMouseButtonDown(0))
                DropShape(Input.mousePosition);
        }
    }

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
            {
                dropped++;
                if (LevelSettings.instance.setting == TitleScreen.Setting.MergeCrown && 150-dropped <= 50)
                {
                    StopCoroutine(FlashWarning(150-dropped));
                    StartCoroutine(FlashWarning(150-dropped));
                    if (150-dropped <= 0)
                        StartCoroutine(OutOfShapes());
                }
                if (LevelSettings.instance.setting == TitleScreen.Setting.Drops && 75-dropped <= 15)
                {
                    StopCoroutine(FlashWarning(75 - dropped));
                    StartCoroutine(FlashWarning(75 - dropped));
                    if (75-dropped <= 0)
                        StartCoroutine(OutOfDrops());
                }
            }

            AudioManager.instance.PlaySound(dropSound, 0.5f);
            UpdateDataText();
            StartCoroutine(GenerateShape(nextShape1, new Vector2(xValue, yValue)));
            RollNextShape();
        }
    }

    IEnumerator OutOfShapes()
    {
        nextImage1.transform.parent.gameObject.SetActive(false);
        nextImage2.transform.parent.gameObject.SetActive(false);
        dataText.transform.parent.gameObject.SetActive(false);
        InputManager.instance.enabled = false;
        yield return new WaitForSeconds(2.5f);
        GameOver("You're Out Of Shapes.", false);
    }

    IEnumerator OutOfDrops()
    {
        nextImage1.transform.parent.gameObject.SetActive(false);
        nextImage2.transform.parent.gameObject.SetActive(false);
        dataText.transform.parent.gameObject.SetActive(false);
        InputManager.instance.enabled = false;
        yield return new WaitForSeconds(2.5f);
        GameOver("You Won!", true);
    }

    IEnumerator FlashWarning(int number)
    {
        AudioManager.instance.PlaySound(timerSound, 0.5f);
        Vector2 zeroSize = new(0, 0);
        Vector2 maxSize = new(1, 1);

        warningText.transform.localScale = zeroSize;
        warningText.text = $"{number}";

        float elapsedTime = 0f;
        float waitTime = 0.5f;
        while (elapsedTime < waitTime)
        {
            warningText.transform.localScale = Vector3.Lerp(zeroSize, maxSize, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        warningText.transform.localScale = maxSize;
    }

    void RollNextShape()
    {
        nextShape1 = nextShape2;
        nextImage1.transform.parent.gameObject.SetActive(true);
        nextImage1.sprite = nextShape2.spriterenderer.sprite;
        nextImage1.color = nextShape2.spriterenderer.color;

        if (toDrop.Count == 0)
        {
            foreach (ChanceOfDrop next in droppedShapes)
            {
                for (int i = 0; i < next.chance; i++)
                    toDrop.Add(next.shape);
            }
        }

        int randRemoval = UnityEngine.Random.Range(0, toDrop.Count);
        nextShape2 = toDrop[randRemoval];
        toDrop.RemoveAt(randRemoval);

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
                nextImage1.rectTransform.sizeDelta = new Vector2(55, 95);
                break;
            case "Inversion":
                nextImage1.rectTransform.sizeDelta = new Vector2(90, 90);
                break;
            case "Wall":
                nextImage1.rectTransform.sizeDelta = new Vector2(110, 50);
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
                nextImage2.rectTransform.sizeDelta = new Vector2(45, 80);
                break;
            case "Inversion":
                nextImage2.rectTransform.sizeDelta = new Vector2(60, 60);
                break;
            case "Wall":
                nextImage2.rectTransform.sizeDelta = new Vector2(65, 25);
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
        else if (LevelSettings.instance.setting == TitleScreen.Setting.MaxDrop)
        {
            char infinitySymbol = '\u221E';
            dataText.text = $"Score: {score}/1500 \nDropped: {dropped}/{infinitySymbol}";
        }
        else if (LevelSettings.instance.setting == TitleScreen.Setting.MergeCrown)
        {
            dataText.text = $"Score: {score} \nDropped: {dropped}/150";
        }
        else if (LevelSettings.instance.setting == TitleScreen.Setting.Drops)
        {
            dataText.text = $"Score: {score}/500 \nDropped: {dropped}/75";
        }

        if (LevelSettings.instance.setting == TitleScreen.Setting.MaxDrop && score >= 1500)
        {
            GameOver("You Lost.", true);
        }
        else if (LevelSettings.instance.setting == TitleScreen.Setting.Drops && score >= 500)
        {
            GameOver("You Lost.", true);
        }
    }

    public void SwitchGravity()
    {
        if (InputManager.instance.enabled)
        {
            AudioManager.instance.PlaySound(gravitySound, 0.5f);
            InputManager.instance.enabled = false;
            nextImage1.transform.parent.gameObject.SetActive(false);
            nextImage2.transform.parent.gameObject.SetActive(false);
            warningText.gameObject.SetActive(false);

            floor.gameObject.SetActive(true);
            ceiling.gameObject.SetActive(true);
            deathLine.gameObject.SetActive(false);

            currentGravity *= -1;
            Shape[] allShapes = FindObjectsByType<Shape>(FindObjectsSortMode.None);
            foreach (Shape shape in allShapes)
                shape.rb.gravityScale = currentGravity;

            StartCoroutine(ArrowAnimation());
            StartCoroutine(UnPauseGame());
        }
    }

    IEnumerator ArrowAnimation()
    {
        Vector2 zeroSize = new(0, 0);
        Vector2 maxSize = new(3, 3);

        Vector3 currRot = gravityArrow.localEulerAngles;
        Vector3 newRot = gravityArrow.localEulerAngles + new Vector3(0, 0, 180);

        gravityArrow.localScale = zeroSize;

        float elapsedTime = 0f;
        float waitTime = 0.5f;
        while (elapsedTime < waitTime)
        {
            gravityArrow.localScale = Vector3.Lerp(zeroSize, maxSize, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gravityArrow.localScale = maxSize;

        elapsedTime = 0f;
        while (elapsedTime < waitTime)
        {
            gravityArrow.localEulerAngles = Vector3.Lerp(currRot, newRot, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gravityArrow.localEulerAngles = newRot;

        elapsedTime = 0f;
        while (elapsedTime < waitTime)
        {
            gravityArrow.localScale = Vector3.Lerp(maxSize, zeroSize, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gravityArrow.localScale = zeroSize;
        warningText.gameObject.SetActive(true);
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
                newPV.Setup(score, shape, 0.75f);
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

            if (won)
                AudioManager.instance.PlaySound(winSound, 0.5f);
            else
                AudioManager.instance.PlaySound(loseSound, 0.5f);

            if (LevelSettings.instance.setting == TitleScreen.Setting.Endless && PlayerPrefs.GetInt($"{SceneManager.GetActiveScene().name} - Endless") < score)
                PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name} - Endless", score);

            else if (LevelSettings.instance.setting == TitleScreen.Setting.MaxDrop && PlayerPrefs.GetInt($"{SceneManager.GetActiveScene().name} - MaxDrop") < dropped)
                PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name} - MaxDrop", dropped);

            else if (won && LevelSettings.instance.setting == TitleScreen.Setting.MergeCrown && PlayerPrefs.GetInt($"{SceneManager.GetActiveScene().name} - Merge") < 150-dropped)
                PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name} - Merge", 150-dropped);

            else if (won && LevelSettings.instance.setting == TitleScreen.Setting.Drops && PlayerPrefs.GetInt($"{SceneManager.GetActiveScene().name} - Drops") > score)
                PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name} - Drops", score);
        }
    }

    void ToggleUI()
    {
        tutorialText.transform.parent.gameObject.SetActive(!tutorialText.transform.parent.gameObject.activeSelf);
    }

#endregion

}
