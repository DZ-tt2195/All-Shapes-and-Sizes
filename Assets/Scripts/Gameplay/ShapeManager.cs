using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.Linq;
public enum CreationType {None, Drop, Merge}
public enum GameState {SettingUp, GameOn, GameOver}
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
        [SerializeField] AudioClip createSound;
        [SerializeField] AudioClip dropSound;
        [SerializeField] AudioClip timerSound;
        [SerializeField] AudioClip winSound;
        [SerializeField] AudioClip loseSound;

    [Foldout("Text", true)]
        [SerializeField] TMP_Text dataText;
        [SerializeField] TMP_Text headerText;
        [SerializeField] TMP_Text tutorialText;
        [SerializeField] TMP_Text warningText;
        [SerializeField] TMP_Text next;
        [SerializeField] TMP_Text giveUp;
        [SerializeField] TMP_Text replay;
        [SerializeField] TMP_Text titleScreen;
        [SerializeField] TMP_Text guideText;

    [Foldout("Tutorial", true)]
        [SerializeField] Image tutorialBackground;
        [SerializeField] List<Image> bonusShapeList = new();
        [SerializeField] List<TMP_Text> bonusTextList = new();
        [SerializeField] Button guideButton;

    [Foldout("Next shapes", true)]
        [SerializeField] Image nextImage1;
        Shape nextShape1;
        [SerializeField] Image nextImage2;
        Shape nextShape2;
        float waitForDrop = 0f;

    [Foldout("To drop", true)]
        [SerializeField] List<ChanceOfDrop> mainShapesToDrop = new();
        [SerializeField] List<ChanceOfDrop> bonusShapesToDrop = new();
        [SerializeField] List<Shape> allShapes = new();
        List<Shape> toDrop = new();
        Dictionary<string, Queue<Shape>> shapeStorage = new();
        HashSet<Shape> shapesInLevel = new();

    [Foldout("Score", true)]
        [SerializeField] int mergeDeath;
        public int score {get; private set;}
        public int dropped {get; private set;}
        public int merged {get; private set;}
        [SerializeField] PointsVisual pv;
        Queue<PointsVisual> visualStorage = new();

    [Foldout("FPS", true)]
        int lastframe = 0;
        int lastupdate = 60;
        float[] framearray = new float[60];
        Stopwatch gameTimer;

    [Foldout("Game end", true)]
        [ReadOnly] public bool mergedCrowns = false;
        [SerializeField] GameObject gameOverTransform;
        [SerializeField] Button resign;
        public GameState state {get; private set;}

    [Foldout("Level geometry", true)]
        [SerializeField] Transform deathLine;
        [SerializeField] Transform floor;
        [SerializeField] Transform ceiling;
        [SerializeField] Transform leftWall;
        [SerializeField] Transform rightWall;
        [SerializeField] Transform gravityArrow;

    #endregion

#region Setup

    private void Awake()
    {
        gameOverTransform.SetActive(false);
        instance = this;
        mainCam = Camera.main;
        Physics2D.gravity = new(0, -10);

        next.text = AutoTranslate.Next();
        giveUp.text = AutoTranslate.Give_Up();
        replay.text = AutoTranslate.Replay();
        titleScreen.text = AutoTranslate.Title_Screen();
        guideText.text = AutoTranslate.Close_Guide();

        foreach (Shape shape in allShapes)
        {
            shapeStorage.Add(shape.GetType().Name, new Queue<Shape>());
        }

        while (bonusShapesToDrop.Count > 3)
        {
            int random = UnityEngine.Random.Range(0, bonusShapesToDrop.Count);
            bonusShapesToDrop.RemoveAt(random);
        }
        tutorialBackground.gameObject.SetActive(true);
        for (int i = 0; i<3; i++)
        {
            Apply(bonusShapeList[i], bonusShapesToDrop[i].shape, true);
            bonusTextList[i].text = Translator.inst.Translate(bonusShapesToDrop[i].shape.GetType().Name);
        }
        guideButton.onClick.AddListener(ClickGuide);
        void ClickGuide()
        {
            if (tutorialBackground.gameObject.activeSelf)
            {
                tutorialBackground.gameObject.SetActive(false);
                guideText.text = AutoTranslate.Open_Guide();
            }
            else
            {
                tutorialBackground.gameObject.SetActive(true);
                guideText.text = AutoTranslate.Close_Guide();                
            }
        }
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

        resign.onClick.AddListener(() => GameOver(AutoTranslate.You_Gave_Up()));
        ceiling.gameObject.SetActive(false);
        deathLine.transform.localPosition = new Vector3(0, ceiling.transform.localPosition.y + 0.15f, 0);

        nextShape2 = AssignRandomShape();
        RollNextShape();

        switch (PrefManager.GetSetting())
        {
            case Setting.Merge_Crown:
                headerText.text = AutoTranslate.Merge_Crown();
                tutorialText.text = AutoTranslate.Merge_Crown_Tutorial(mergeDeath.ToString());
                break;
            case Setting.Endless:
                headerText.text = AutoTranslate.Endless();
                tutorialText.text = AutoTranslate.Endless_Tutorial();
                break;
        }

        nextImage1.transform.parent.gameObject.SetActive(false);
        nextImage2.transform.parent.gameObject.SetActive(false);

        StartCoroutine(DropRandomly(typeof(Circle), 75));
        StartCoroutine(BeginGame());
        IEnumerator BeginGame()
        {
            yield return new WaitForSeconds(6f);
            while (tutorialBackground.gameObject.activeSelf)
                yield return null;
            if (state == GameState.GameOver)
                yield break;
            
            state = GameState.GameOn;
            NewVisual(AutoTranslate.Begin(), 3, Vector3.zero, Color.white);
            AudioManager.instance.PlaySound(winSound, 0.2f);
            InputManager.instance.enabled = true;
            dataText.transform.parent.gameObject.SetActive(true);
        
            gameTimer = new Stopwatch();
            gameTimer.Start();
            ShapeUI();
        }
    }

#endregion

#region Shapes

    private void Update()
    {
        waitForDrop -= Time.deltaTime;
        if (waitForDrop <= 0f && InputManager.instance.enabled && !Application.isMobilePlatform)
        {
            if (Input.GetMouseButtonDown(0) && state == GameState.GameOn)
            {
                waitForDrop = 0.15f;
                DropShape(Input.mousePosition);
            }
        }

        if (dropped == 0) score = 0;

        string answer = gameTimer == null ? AutoTranslate.Time("0:00:00") : AutoTranslate.Time(MyExtensions.StopwatchTime(gameTimer));
        answer += $"\n{AutoTranslate.FPS(CalculateFrames())}\n";
        char infinitySymbol = '\u221E';
        switch (PrefManager.GetSetting())
        {
            case Setting.Endless:
                answer += AutoTranslate.Score_Text(score.ToString());
                answer += $"\n{AutoTranslate.Drop_Text(dropped.ToString(), infinitySymbol.ToString())}";
                break;
            case Setting.Merge_Crown:
                answer += AutoTranslate.Score_Text(score.ToString());
                answer += $"\n{AutoTranslate.Drop_Text(dropped.ToString(), mergeDeath.ToString())}";
                break;
        }
        dataText.text = answer;

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

        float xValue = GetWorldCoordinates(screenPosition).x;
        if (xValue > XSpawnRange().Item1 && xValue < XSpawnRange().Item2)
        {
            dropped++;
            if (PrefManager.GetSetting() == Setting.Merge_Crown && mergeDeath-dropped <= 50)
            {
                StopCoroutine(FlashWarning(mergeDeath - dropped));
                StartCoroutine(FlashWarning(mergeDeath - dropped));
                if (mergeDeath - dropped <= 0)
                    StartCoroutine(WaitForEnd(AutoTranslate.Game_Over()));
            }

            GenerateShape(nextShape1.GetType().Name, new Vector2(xValue, YSpawn()), CreationType.Drop);
            RollNextShape();
        }
    }
    IEnumerator WaitForEnd(string message)
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
        Apply(nextImage1, nextShape1, true);
        Apply(nextImage2, nextShape2, false);
    }
    void Apply(Image image, Shape shape, bool large)
    {
        image.transform.parent.gameObject.SetActive(true);
        image.sprite = shape.spriterenderer.sprite;
        image.color = shape.spriterenderer.color;
        image.rectTransform.sizeDelta = shape.UISize(large);
    }
    public float YSpawn()
    {
        return (Physics2D.gravity.y > 0) ? deathLine.position.y + 0.25f : deathLine.position.y - 0.25f;
    }
    public (float, float) XSpawnRange()
    {
        return (leftWall.position.x + 0.5f, rightWall.position.x - 0.5f);
    }
    void RollNextShape()
    {
        nextShape1 = nextShape2;
        do
        {
            nextShape2 = AssignRandomShape();
        } while (nextShape1.name.Equals(typeof(Inverter).Name) && nextShape2.name.Equals(typeof(Inverter).Name));
        ShapeUI();
    }
    Shape AssignRandomShape()
    {
        if (toDrop.Count == 0)
        {
            foreach (ChanceOfDrop next in mainShapesToDrop)
            {
                for (int i = 0; i < next.chance; i++)
                    toDrop.Add(next.shape);
            }
            foreach (ChanceOfDrop next in bonusShapesToDrop)
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
    public void GenerateShape(string shape, Vector2 spawn, CreationType creationType)
    {
        if (state == GameState.GameOver) 
            return;

        Shape toCreate = null;
        if (shapeStorage[shape].Count > 0)
            toCreate = shapeStorage[shape].Dequeue();
        else
            toCreate = Instantiate(allShapes.FirstOrDefault(s => s.GetType().Name.Equals(shape)));

        switch (creationType)
        {
            case CreationType.Drop:
                AudioManager.instance.PlaySound(dropSound, 0.25f); 
                break;
            case CreationType.Merge:
                AudioManager.instance.PlaySound(createSound, 0.25f); 
                merged++;
                break;
            case CreationType.None:
                break;
        }
        shapesInLevel.Add(toCreate);
        toCreate.Setup(spawn);
    }
    public void ReturnShape(Shape shape)
    {
        shape.canInteract = false;
        shapeStorage[shape.GetType().Name].Enqueue(shape);
        shapesInLevel.Remove(shape);
        shape.gameObject.SetActive(false);
    }
    public IEnumerator DropRandomly(Type shapeToSpawn, int numDrop)
    {
        for (int i = 0; i < numDrop; i++)
        {
            yield return new WaitForSeconds(0.05f);
            GenerateShape(shapeToSpawn.Name, new Vector2(RandomX(), YSpawn()), CreationType.Drop);

            float RandomX()
            {
                return UnityEngine.Random.Range(XSpawnRange().Item1, XSpawnRange().Item2);
            }
        }
    }

#endregion

#region Other
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
        if (state == GameState.GameOn)
        {
            PointsVisual newVisual = (visualStorage.Count > 0) ? visualStorage.Dequeue() : Instantiate(pv);
            newVisual.Setup(text, spawn, 0.75f, size, textColor);
        }
    }
    public void ReturnVisual(PointsVisual visual)
    {
        visualStorage.Enqueue(visual);
        visual.gameObject.SetActive(false);
    }
    public void GameOver(string loseMessage)
    {
        if (state != GameState.GameOver)
        {
            gameTimer?.Stop();
            InputManager.instance.enabled = false;
            tutorialBackground.gameObject.SetActive(false);
            gameOverTransform.SetActive(true);
            state = GameState.GameOver;

            bool won = false;
            Setting currentSetting = PrefManager.GetSetting();

            if (currentSetting == Setting.Merge_Crown)
            {
                won = mergedCrowns;
                if (won && PrefManager.GetScore(currentSetting) < mergeDeath - dropped)
                    PrefManager.SetScore(currentSetting, mergeDeath-dropped);
            }
            else if (currentSetting == Setting.Endless)
            {
                if (PrefManager.GetScore(currentSetting) < score)
                    PrefManager.SetScore(currentSetting, score);
            }

            TMP_Text textBox = gameOverTransform.transform.GetChild(0).GetComponent<TMP_Text>();
            if (won)
            {
                AudioManager.instance.PlaySound(winSound, 0.5f);
                textBox.text = AutoTranslate.You_Won();
            }
            else
            {
                AudioManager.instance.PlaySound(loseSound, 0.5f);
                textBox.text = loseMessage;
            }
        }
    }
    public Transform GetGravityArrow() => gravityArrow;
    public void SwitchGravity()
    {
        Physics2D.gravity = new Vector2(0, Physics2D.gravity.y*-1);
        if (Physics2D.gravity.y > 0)
        {
            deathLine.transform.localPosition = new Vector3(0, floor.transform.localPosition.y - 0.25f, 0);
            ceiling.gameObject.SetActive(true);
            floor.gameObject.SetActive(false);
        }
        else
        {
            deathLine.transform.localPosition = new Vector3(0, ceiling.transform.localPosition.y + 0.25f, 0);
            floor.gameObject.SetActive(true);
            ceiling.gameObject.SetActive(false);
        }
    }
#endregion

}
