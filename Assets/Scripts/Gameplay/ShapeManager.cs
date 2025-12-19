using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
[Serializable]
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
        [SerializeField] Image saveImage;
        Shape savedShape;

    [Foldout("To drop", true)]
        [SerializeField] List<ChanceOfDrop> shapesToDrop = new();
        Shape[] allShapes;
        List<Shape> toDrop = new();
        [SerializeField] Transform gravityArrow;
        float currentGravity = 2.5f;
        Dictionary<KindOfShape, Queue<Shape>> shapeStorage = new();

    [Foldout("Score", true)]
        int score = 0;
        int dropped = 0;
        [SerializeField] PointsVisual pv;
        Queue<PointsVisual> visualStorage = new();

        [SerializeField] Button hideUI;

    [Foldout("Numbers", true)]
        int mergeDeath = 150;
        int dropDeath = 250;
        int dropCreate = 50;
        int permaDeath = 1500;

    [Foldout("FPS", true)]
        int lastframe = 0;
        int lastupdate = 60;
        float[] framearray = new float[60];
        Stopwatch gameTimer;

    [Foldout("Game end", true)]
        [ReadOnly] public bool mergedCrowns = false;
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

        allShapes = Resources.LoadAll<Shape>("Shapes");
        foreach (Shape shape in allShapes)
            shapeStorage.Add(shape.myShape, new Queue<Shape>());
    }

    private void OnEnable()
    {
        if (Application.isMobilePlatform)
            InputManager.instance.OnStartTouch += DropShape;
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

        resign.onClick.AddListener(() => GameOver(ToTranslate.You_Gave_Up));
        hideUI.onClick.AddListener(ToggleUI);
        void ToggleUI() { tutorialText.transform.parent.gameObject.SetActive(!tutorialText.transform.parent.gameObject.activeSelf);}

        ceiling.gameObject.SetActive(false);
        deathLine.transform.localPosition = new Vector3(0, ceiling.transform.localPosition.y + 0.15f, 0);

        nextShape2 = AssignRandomShape();
        RollNextShape();

        string answer = $"{AutoTranslate.DoEnum(ToTranslate.Tutorial_1)}\n" +
        $"{(Application.isEditor ? AutoTranslate.DoEnum(ToTranslate.Tutorial_2) + "\n" : "")}" +
            $"{AutoTranslate.DoEnum(ToTranslate.Tutorial_3)}\n\n";
        switch (PrefManager.GetSetting())
        {
            case Setting.MergeCrown:
                answer += $"{AutoTranslate.Merge_Crown_Tutorial(mergeDeath.ToString())}\n";
                answer += $"{AutoTranslate.DoEnum(ToTranslate.Merge_Crown_WinCon)}";
                break;
            case Setting.DropShape:
                answer += $"{AutoTranslate.Drop_Shape_Tutorial(dropDeath.ToString())}\n";
                answer += $"{AutoTranslate.Drop_Shape_WinCon(dropCreate.ToString())}";
                break;
            case Setting.DropEndless:
                answer += $"{AutoTranslate.Drop_Endless_Tutorial(permaDeath.ToString())}\n";
                answer += $"{AutoTranslate.DoEnum(ToTranslate.Endless_WinCon)}";
                break;
            case Setting.MergeEndless:
                answer += $"{AutoTranslate.DoEnum(ToTranslate.Merge_Endless_Tutorial)}\n";
                answer += $"{AutoTranslate.DoEnum(ToTranslate.Endless_WinCon)}";
                break;
        }
        tutorialText.text = answer;

        StartCoroutine(DropRandomly());
    }

    IEnumerator DropRandomly()
    {
        dataText.transform.parent.gameObject.SetActive(false);
        nextImage1.transform.parent.gameObject.SetActive(false);
        nextImage2.transform.parent.gameObject.SetActive(false);
        saveImage.transform.parent.gameObject.SetActive(false);

        for (int i = 0; i < 75; i++)
        {
            yield return new WaitForSeconds(0.05f);
            AudioManager.instance.PlaySound(dropSound, 0.2f);
            GenerateShape(KindOfShape.Circle, new Vector2(UnityEngine.Random.Range(leftWall.position.x + 0.6f, rightWall.position.x - 0.6f), deathLine.position.y - 0.15f));
        }

        yield return new WaitForSeconds(2f);
        NewVisual(Translator.inst.Translate("Begin"), 3, Vector3.zero, Color.white);
        AudioManager.instance.PlaySound(winSound, 0.5f);

        yield return new WaitForSeconds(1f);

        if (!hasEnded)
        {
            dataText.transform.parent.gameObject.SetActive(true);
            InputManager.instance.enabled = true;
            dropped = 0;
            gameTimer = new Stopwatch();
            gameTimer.Start();
            ShapeUI();
        }
    }

#endregion

#region Shape Info

    private void Update()
    {
        if (InputManager.instance.enabled && !Application.isMobilePlatform)
        {
            if (Input.GetMouseButtonDown(0) && !hasEnded)
                DropShape(Input.mousePosition);
            else if (Input.GetMouseButtonDown(1) && !hasEnded && Application.isEditor)
                SaveShape();
        }

        if (dropped == 0)
            score = 0;

        if (!dataText.transform.parent.gameObject.activeSelf)
            return;

        dataText.text = AutoTranslate.Time(MyExtensions.StopwatchTime(gameTimer));
        dataText.text += $"\n{AutoTranslate.FPS(CalculateFrames())}\n";
        char infinitySymbol = '\u221E';
        switch (PrefManager.GetSetting())
        {
            case Setting.MergeEndless:
                dataText.text += AutoTranslate.Score_Text_No_Limit(score.ToString());
                dataText.text += $"\n{AutoTranslate.Drop_Text(dropped.ToString(), infinitySymbol.ToString())}";
                break;
            case Setting.DropEndless:
                dataText.text += AutoTranslate.Score_Text(score.ToString(), permaDeath.ToString());
                dataText.text += $"\n{AutoTranslate.Drop_Text(dropped.ToString(), infinitySymbol.ToString())}";
                if (score >= permaDeath)
                    GameOver(ToTranslate.You_Lost);
                break;
            case Setting.MergeCrown:
                dataText.text += AutoTranslate.Score_Text_No_Limit(score.ToString());
                dataText.text += $"\n{AutoTranslate.Drop_Text(dropped.ToString(), mergeDeath.ToString())}";
                break;
            case Setting.DropShape:
                dataText.text += AutoTranslate.Score_Text(score.ToString(), dropDeath.ToString());
                dataText.text += $"\n{AutoTranslate.Drop_Text(dropped.ToString(), dropCreate.ToString())}";
                if (score >= dropDeath)
                    GameOver(ToTranslate.You_Lost);
                break;
        }

        string CalculateFrames()
        {
            framearray[lastframe] = Time.deltaTime;
            lastframe = (lastframe + 1);
            if (lastframe == 60)
            {
                lastframe = 0;
                float total = 0;
                for (int i = 0; i < framearray.Length; i++)
                    total += framearray[i];
                lastupdate = (int)(framearray.Length / total);
                return lastupdate.ToString();
            }
            return (lastupdate > Application.targetFrameRate) ? Application.targetFrameRate.ToString() : lastupdate.ToString();
        }
    }

    void DropShape(Vector2 screenPosition)
    {
        Vector2 GetWorldCoordinates(Vector2 screenPos)
        {
            Vector2 screenCoord = new(screenPos.x, screenPos.y);
            Vector2 worldCoord = mainCam.ScreenToWorldPoint(screenCoord);
            return worldCoord;
        }

        float yValue = (currentGravity > 0) ? deathLine.position.y - 0.6f : deathLine.position.y + 0.6f;
        float xValue = GetWorldCoordinates(screenPosition).x;

        if (xValue > (leftWall.position.x + 0.3f) && xValue < (rightWall.position.x - 0.3f))
        {
            if (nextShape1.IsMainShape())
            {
                dropped++;
                if (PrefManager.GetSetting() == Setting.MergeCrown && mergeDeath-dropped <= 50)
                {
                    StopCoroutine(FlashWarning(mergeDeath - dropped));
                    StartCoroutine(FlashWarning(mergeDeath - dropped));
                    if (mergeDeath - dropped <= 0)
                        StartCoroutine(WaitForEnd(ToTranslate.Out_of_Shapes));
                }
                else if (PrefManager.GetSetting() == Setting.DropShape && dropCreate-dropped <= 15)
                {
                    StopCoroutine(FlashWarning(dropCreate - dropped));
                    StartCoroutine(FlashWarning(dropCreate - dropped));
                    if (dropCreate - dropped <= 0)
                        StartCoroutine(WaitForEnd(ToTranslate.Blank));
                }
            }

            AudioManager.instance.PlaySound(dropSound, 0.5f);
            GenerateShape(nextShape1.myShape, new Vector2(xValue, yValue));
            RollNextShape();
        }
    }

    void SaveShape()
    {
        if (savedShape == null)
        {
            savedShape = nextShape1;
            nextShape1 = null;
            RollNextShape();
        }
        else
        {
            (nextShape1, savedShape) = (savedShape, nextShape1);
            ShapeUI();
        }
    }

    IEnumerator WaitForEnd(ToTranslate message)
    {
        nextImage1.transform.parent.gameObject.SetActive(false);
        nextImage2.transform.parent.gameObject.SetActive(false);
        InputManager.instance.enabled = false;
        yield return new WaitForSeconds(2.5f);
        GameOver(message);
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

    void ShapeUI()
    {
        void Apply(Image image, Shape shape, bool large)
        {
            image.transform.parent.gameObject.SetActive(true);
            image.sprite = shape.spriterenderer.sprite;
            image.color = shape.spriterenderer.color;

            switch (shape.myShape)
            {
                case KindOfShape.Circle:
                    image.rectTransform.sizeDelta = large ? new(50, 50) : new(40, 40);
                    break;
                case KindOfShape.Square:
                    image.rectTransform.sizeDelta = large ? new(65, 65) : new(50, 50);
                    break;
                case KindOfShape.Arrow:
                    image.rectTransform.sizeDelta = large ? new(100, 100) : new(70, 70);
                    break;
                case KindOfShape.Diamond:
                    image.rectTransform.sizeDelta = large ? new(110, 60) : new(80, 50);
                    break;
                case KindOfShape.Bomb:
                    image.rectTransform.sizeDelta = large ? new(55, 95) : new(45, 80);
                    break;
                case KindOfShape.Inversion:
                    image.rectTransform.sizeDelta = large ? new(90, 90) : new(60, 60);
                    break;
                case KindOfShape.Wall:
                    image.rectTransform.sizeDelta = large ? new(110, 50) : new(65, 25);
                    break;
            }
        }

        Apply(nextImage1, nextShape1, true);
        Apply(nextImage2, nextShape2, false);
        if (savedShape != null)
            Apply(saveImage, savedShape, true);
    }

    #endregion

#region New Shapes

    void RollNextShape()
    {
        nextShape1 = nextShape2;
        do
        {
            nextShape2 = AssignRandomShape();
        } while (nextShape1.name.Equals("Inversion") && nextShape2.name.Equals("Inversion"));
        ShapeUI();
    }

    Shape AssignRandomShape()
    {
        if (toDrop.Count == 0)
        {
            foreach (ChanceOfDrop next in shapesToDrop)
            {
                for (int i = 0; i < next.chance; i++)
                    toDrop.Add(next.shape);
            }
        }
        int randIndex = UnityEngine.Random.Range(0, toDrop.Count);
        Shape shape = toDrop[randIndex];
        toDrop.RemoveAt(randIndex);
        return shape;
    }

    public void GenerateShape(KindOfShape shape, Vector3 spawn)
    {
        Shape toCreate = null;
        if (shapeStorage[shape].Count > 0)
        {
            toCreate = shapeStorage[shape].Dequeue();
        }
        else
        {
            foreach (Shape drop in allShapes)
            {
                if (drop.myShape == shape)
                {
                    toCreate = Instantiate(drop);
                    break;
                }
            }
        }
        toCreate.Setup(spawn, currentGravity);
    }

    public void ReturnShape(Shape shape)
    {
        shapeStorage[shape.myShape].Enqueue(shape);
        shape.gameObject.SetActive(false);
    }

#endregion

#region Other

    public void SwitchGravity()
    {
        if (InputManager.instance.enabled)
        {
            AudioManager.instance.PlaySound(gravitySound, 0.5f);
            InputManager.instance.enabled = false;
            nextImage1.transform.parent.gameObject.SetActive(false);
            nextImage2.transform.parent.gameObject.SetActive(false);
            saveImage.transform.parent.gameObject.SetActive(false);
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

        if (!hasEnded)
        {
            InputManager.instance.enabled = true;
            deathLine.gameObject.SetActive(true);
            ShapeUI();
        }
    }

    public void AddScore(int toAdd, Vector3 spawn, Color textColor)
    {
        if (dropped > 0)
        {
            this.score += toAdd;
            NewVisual($"+{toAdd}", (int)Mathf.Sqrt(toAdd), spawn, textColor);
        }
    }

    void NewVisual(string text, int size, Vector3 spawn, Color textColor)
    {
        PointsVisual newVisual = (visualStorage.Count > 0) ? visualStorage.Dequeue() : Instantiate(pv);
        newVisual.Setup(text, spawn, 0.75f, size, textColor);
    }

    public void ReturnVisual(PointsVisual visual)
    {
        visualStorage.Enqueue(visual);
        visual.gameObject.SetActive(false);
    }

    public void GameOver(ToTranslate loseMessage)
    {
        if (!hasEnded)
        {
            gameTimer.Stop();
            InputManager.instance.enabled = false;
            gameOverTransform.SetActive(true);
            hasEnded = true;

            bool won = false;
            Setting currentSetting = PrefManager.GetSetting();
            ToTranslate currentLevel = PrefManager.GetLevel(); 

            if (currentSetting == Setting.MergeCrown)
            {
                won = mergedCrowns;
                if (won && PrefManager.GetScore(currentLevel, currentSetting) < mergeDeath - dropped)
                    PrefManager.SetScore(currentLevel, currentSetting, mergeDeath-dropped);
            }
            else if (currentSetting == Setting.DropShape)
            {
                won = (dropped == dropCreate) && score < dropDeath;
                if (won && PrefManager.GetScore(currentLevel, currentSetting) > score)
                    PrefManager.SetScore(currentLevel, currentSetting, score);
            }
            else if (currentSetting == Setting.DropEndless && PrefManager.GetScore(currentLevel, currentSetting) < dropped)
            {
                PrefManager.SetScore(currentLevel, currentSetting, dropped);
            }            
            else if (currentSetting == Setting.MergeEndless && PrefManager.GetScore(currentLevel, currentSetting) < score)
            {
                PrefManager.SetScore(currentLevel, currentSetting, score);
            }

            TMP_Text textBox = gameOverTransform.transform.GetChild(0).GetComponent<TMP_Text>();
            if (won)
            {
                AudioManager.instance.PlaySound(winSound, 0.5f);
                textBox.text = AutoTranslate.DoEnum(ToTranslate.You_Won);
            }
            else
            {
                AudioManager.instance.PlaySound(loseSound, 0.5f);
                textBox.text = AutoTranslate.DoEnum(loseMessage);
            }
        }
    }

#endregion

}
