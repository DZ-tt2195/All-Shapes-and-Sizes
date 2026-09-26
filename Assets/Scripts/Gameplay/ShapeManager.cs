using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.Linq;
public enum ColumnDrop {Top, Bottom}
public enum CreationType {Drop, Combine, Other}
public enum GameState {SettingUp, GameOn, GameOver}
[Serializable]
public class SpawnColumn
{
    int columnNumber; public int ColumnNumber() => columnNumber;
    [SerializeField] Transform parent;
    List<SpawnButton> listOfButtons = new();
    public void Setup(int columnNumber)
    {
        this.columnNumber = columnNumber;
        foreach (Transform child in parent)
        {
            SpawnButton button = child.GetComponent<SpawnButton>();
            listOfButtons.Add(button);
            button.Setup(columnNumber);
        }
    }
    public void EnableButtons(FindNumber toFind, int number)
    {
        for (int i = 0; i<listOfButtons.Count; i++)
            listOfButtons[i].gameObject.SetActive(MyExtensions.Comparison(toFind, i, number));
    }
    public int MaxCount() => listOfButtons.Count-1;
    public void EnableAll() => EnableButtons(FindNumber.Minimum, -1);
    public void DisableAll() => EnableButtons(FindNumber.Exact, -1);
    public SpawnButton First() => listOfButtons[0];
    public SpawnButton Last() => listOfButtons[MaxCount()];
}

public class ShapeManager : MonoBehaviour
{

#region Variables

    public static ShapeManager inst;
    Camera mainCam;
    public GameState state {get; private set;}

    [Foldout("Audio", true)]
        [SerializeField] AudioClip createSound;
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
        [SerializeField] Button resign;

    [Foldout("Guide", true)]
        [SerializeField] Image tutorialBackground;
        [SerializeField] Button guideButton;
        Vector3 guideOriginal;
        [SerializeField] TMP_Text guideText;
        [SerializeField] List<ShapeDisplay> displaysOnScreen;

    [Foldout("Shapes", true)]
        public static ColumnDrop dropState {get; private set;}
        [SerializeField] List<SpawnColumn> listOfSpawnColumns = new();
        [SerializeField] List<Image> nextImages = new();
        List<Shape> nextShapesToDrop = new();
        float waitForDrop = 0f;
        HashSet<Shape> selectedBonusShapes;
        HashSet<Shape> willReact = new();
        Dictionary<string, Queue<Shape>> shapeStorage = new();
        Dictionary<string, HashSet<Shape>> allShapesInLevel = new(); public Dictionary<string, HashSet<Shape>> GetExistingShapes => allShapesInLevel;

    [Foldout("Score", true)]
        [ReadOnly] public bool mergedCrowns = false;
        [SerializeField] int combineCrownGameOver;
        int score;
        int dropped;
        [SerializeField] PointsVisual pv;
        Queue<PointsVisual> visualStorage = new();

    [Foldout("FPS", true)]
        int lastframe = 0;
        int lastupdate = 60;
        float[] framearray = new float[60];
        Stopwatch gameTimer;

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
        inst = this;
        mainCam = Camera.main;

        Physics2D.gravity = new(0, -10);
        dropState = ColumnDrop.Top;
        warningText.transform.localScale = new Vector2(0, 0);
        //InputManager.instance.enabled = false;
        gravityArrow.transform.localScale = new Vector2(0, 0);
        gravityArrow.transform.localEulerAngles = new Vector3(0, 0, -90);

        next.text = AutoTranslate.Next();
        giveUp.text = AutoTranslate.Give_Up();
        replay.text = AutoTranslate.Replay();
        titleScreen.text = AutoTranslate.Title_Screen();

        resign.onClick.AddListener(() => GameOver(AutoTranslate.You_Gave_Up()));
        ceiling.gameObject.SetActive(false);
        deathLine.transform.localPosition = new Vector3(0, ceiling.transform.localPosition.y + 0.15f, 0);
        foreach (Image image in nextImages)
            image.transform.parent.gameObject.SetActive(false);
        for (int i = 0; i<listOfSpawnColumns.Count; i++)
            listOfSpawnColumns[i].Setup(i);

        switch (PrefManager.GetMode())
        {
            case GameMode.Combine_Crown:
                headerText.text = AutoTranslate.Combine_Crown();
                tutorialText.text = AutoTranslate.Combine_Crown_Tutorial(combineCrownGameOver.ToString());
                break;
            case GameMode.Endless:
                headerText.text = AutoTranslate.Endless();
                tutorialText.text = AutoTranslate.Endless_Tutorial();
                break;
        }

        guideOriginal = guideButton.transform.localPosition;
        guideButton.onClick.AddListener(ClickGuide);
        tutorialBackground.gameObject.SetActive(false);
        ClickGuide();

        void ClickGuide()
        {
            if (tutorialBackground.gameObject.activeSelf)
            {
                tutorialBackground.gameObject.SetActive(false);
                guideText.text = AutoTranslate.Open_Guide();
                guideButton.transform.localPosition = guideOriginal;
            }
            else
            {
                tutorialBackground.gameObject.SetActive(true);
                guideText.text = AutoTranslate.Close_Guide();                
                guideButton.transform.localPosition = Vector3.zero;
            }
        }
    }
    /*
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
    */
    private void Start()
    {
        selectedBonusShapes = GameFiles.inst.SavedBonusShapes();
        List<Shape> selectedShapes = selectedBonusShapes.ToList();
        for (int i = 0; i<selectedShapes.Count; i++)
            displaysOnScreen[i].AssignShape(selectedShapes[i]);

        StartCoroutine(DropRandomly(typeof(Circle), 75, false));
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
            //InputManager.instance.enabled = true;
            foreach (SpawnColumn column in listOfSpawnColumns)
                column.EnableAll();
            dataText.transform.parent.gameObject.SetActive(true);
            
            FutureShapes();        
            gameTimer = new Stopwatch();
            gameTimer.Start();
        }
    }

#endregion

#region New Shapes

    private void Update()
    {
        waitForDrop -= Time.deltaTime;
        if (dropped == 0) score = 0;

        string answer = gameTimer == null ? AutoTranslate.Time("0:00:00") : AutoTranslate.Time(MyExtensions.StopwatchTime(gameTimer));
        answer += $"\n{AutoTranslate.FPS(CalculateFrames())}\n";
        char infinitySymbol = '\u221E';
        switch (PrefManager.GetMode())
        {
            case GameMode.Endless:
                answer += AutoTranslate.Score_Text(score.ToString());
                answer += $"\n{AutoTranslate.Drop_Text(dropped.ToString(), infinitySymbol.ToString())}";
                break;
            case GameMode.Combine_Crown:
                answer += AutoTranslate.Score_Text(score.ToString());
                answer += $"\n{AutoTranslate.Drop_Text(dropped.ToString(), combineCrownGameOver.ToString())}";
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
    public void DropNewShape(Vector2 screenPosition, int columnNumber)
    {
        if (waitForDrop <= 0f && state == GameState.GameOn && !tutorialBackground.gameObject.activeSelf)
        {
            dropped++;
            waitForDrop = 0.15f;

            if (PrefManager.GetMode() == GameMode.Combine_Crown && combineCrownGameOver-dropped <= 50)
            {
                StopCoroutine(FlashWarning(combineCrownGameOver - dropped));
                StartCoroutine(FlashWarning(combineCrownGameOver - dropped));
                if (combineCrownGameOver - dropped <= 0)
                    StartCoroutine(WaitForEnd(AutoTranslate.Game_Over()));
            }
            Vector2 spawn = screenPosition;
            switch (dropState)
            {
                case ColumnDrop.Top:
                    spawn = listOfSpawnColumns[columnNumber].First().transform.position;
                    break;
                case ColumnDrop.Bottom:
                    spawn = listOfSpawnColumns[columnNumber].Last().transform.position;
                    break;
            }
            Vector3 UIToWorld(Vector3 position)
            {
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null,position);
                screenPos.z = Mathf.Abs(mainCam.transform.position.z);
                return mainCam.ScreenToWorldPoint(screenPos);
            }

            GenerateShape(nextShapesToDrop[0].GetType().Name, UIToWorld(spawn), CreationType.Drop);
            nextShapesToDrop.RemoveAt(0);
            FutureShapes();
        }
    }
    void FutureShapes()
    {
        if (nextShapesToDrop.Count < nextImages.Count)
        {
            UnityEngine.Debug.Log("shuffling bag of shapes");
            List<Shape> upcomingShapes = new();
            foreach (Shape shape in GameFiles.inst.AllMains())
            {
                for (int i = 0; i < shape.DropChance; i++)
                    upcomingShapes.Add(shape);
            }
            foreach (Shape shape in selectedBonusShapes)
            {
                for (int i = 0; i < shape.DropChance; i++)
                    upcomingShapes.Add(shape);
            }
            upcomingShapes = upcomingShapes.Shuffle();
            nextShapesToDrop.AddRange(upcomingShapes);
        }
        for (int i = 0 ; i<nextImages.Count; i++)
            ApplySprite(nextImages[i], nextShapesToDrop[i], i == 0);
    }
    public void GenerateShape(string shape, Vector2 spawn, CreationType creationType, bool cursed = false)
    {
        if (state == GameState.GameOver) 
            return;

        Shape toCreate = null;
        if (!shapeStorage.ContainsKey(shape))
            shapeStorage.Add(shape, new Queue<Shape>());
        if (!allShapesInLevel.ContainsKey(shape))
            allShapesInLevel.Add(shape, new HashSet<Shape>());

        if (shapeStorage[shape].Count > 0)
            toCreate = shapeStorage[shape].Dequeue();
        else
            toCreate = Instantiate(GameFiles.inst.GetShape(shape));

        switch (creationType)
        {
            case CreationType.Drop:
                AudioManager.instance.Menu(); 
                break;
            case CreationType.Other:
                AudioManager.instance.Menu(); 
                break;
            case CreationType.Combine:
                AudioManager.instance.PlaySound(createSound, 0.25f); 
                break;
        }
        allShapesInLevel[shape].Add(toCreate);
        foreach (Shape reacting in new HashSet<Shape>(willReact))
        {
            if (reacting.gameObject.activeSelf)
                reacting.OnNewShape(toCreate, creationType);
        }
        if (toCreate.TrackNewShapes())
            willReact.Add(toCreate);
        toCreate.Setup(spawn, cursed);
    }
    public void ReturnShape(Shape shape)
    {
        shape.canInteract = false;
        shapeStorage[shape.GetType().Name].Enqueue(shape);
        allShapesInLevel[shape.GetType().Name].Remove(shape);
        willReact.Remove(shape);
        shape.gameObject.SetActive(false);
    }
    public IEnumerator DropRandomly(Type shapeToSpawn, int numDrop, bool cursed)
    {
        for (int i = 0; i < numDrop; i++)
        {
            yield return new WaitForSeconds(0.05f);
            GenerateShape(shapeToSpawn.Name, new Vector2(RandomX(), YSpawn()), CreationType.Drop, cursed);

            float RandomX()
            {
                return UnityEngine.Random.Range(XSpawnRange().Item1, XSpawnRange().Item2);
            }
            float YSpawn()
            {
                return dropState == ColumnDrop.Top ? YSpawnRange().Item2 : YSpawnRange().Item1;
            }
        }
    }

#endregion

#region UI
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
            //InputManager.instance.enabled = false;
            tutorialBackground.gameObject.SetActive(true);
            guideButton.gameObject.SetActive(false);
            replay.transform.parent.gameObject.SetActive(true);
            titleScreen.transform.parent.gameObject.SetActive(true);
            state = GameState.GameOver;

            bool won = false;
            GameMode currentSetting = PrefManager.GetMode();

            if (currentSetting == GameMode.Combine_Crown)
            {
                won = mergedCrowns;
                if (won)
                {
                    if (PrefManager.GetScore(currentSetting) < 0 || PrefManager.GetScore(currentSetting) > dropped)
                        PrefManager.SetScore(currentSetting, dropped);
                }
            }
            else if (currentSetting == GameMode.Endless)
            {
                if (PrefManager.GetScore(currentSetting) < score)
                    PrefManager.SetScore(currentSetting, score);
            }
            PlayerPrefs.Save();

            if (won)
            {
                AudioManager.instance.PlaySound(winSound, 0.5f);
                tutorialText.text = AutoTranslate.You_Won(dropped.ToString());
            }
            else
            {
                AudioManager.instance.PlaySound(loseSound, 0.5f);
                tutorialText.text = loseMessage;
            }
        }
    }
    public Transform GetGravityArrow() => gravityArrow;
    public void SwitchGravity()
    {
        if (dropState == ColumnDrop.Top)
        {
            deathLine.transform.localPosition = new Vector3(0, floor.transform.localPosition.y - 0.25f, 0);
            ceiling.gameObject.SetActive(true);
            floor.gameObject.SetActive(false);
            Physics2D.gravity = new Vector2(0, Mathf.Abs(Physics2D.gravity.y));
            dropState = ColumnDrop.Bottom;
        }
        else if (dropState == ColumnDrop.Bottom)
        {
            deathLine.transform.localPosition = new Vector3(0, ceiling.transform.localPosition.y + 0.25f, 0);
            ceiling.gameObject.SetActive(false);
            floor.gameObject.SetActive(true);
            Physics2D.gravity = new Vector2(0, -1*Mathf.Abs(Physics2D.gravity.y));
            dropState = ColumnDrop.Top;
        }
    }
    IEnumerator WaitForEnd(string message)
    {
        foreach (Image image in nextImages)
            image.transform.parent.gameObject.SetActive(false);
        foreach (SpawnColumn column in listOfSpawnColumns)
            column.DisableAll();
        //InputManager.instance.enabled = false;
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
    public static void ApplySprite(Image image, Shape shape, bool large)
    {
        image.transform.parent.gameObject.SetActive(true);
        image.sprite = shape.spriterenderer.sprite;
        image.color = shape.spriterenderer.color;
        image.rectTransform.sizeDelta = shape.UISize(large);
    }
    public (float, float) XSpawnRange()
    {
        return (leftWall.position.x + 0.5f, rightWall.position.x - 0.5f);
    }
    public (float, float) YSpawnRange()
    {
        return (floor.position.y + 0.5f, ceiling.position.y - 0.5f);
    }
#endregion

}