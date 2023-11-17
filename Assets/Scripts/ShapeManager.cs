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
    int score = 0;
    int dropped = 0;

    [ReadOnly] public Camera mainCam;
    [SerializeField] List<Shape> listOfShapes = new List<Shape>();

    private void Awake()
    {
        instance = this;
        dataText = GameObject.Find("Data Text").GetComponent<TMP_Text>();
        mainCam = Camera.main;
    }

    private void OnEnable()
    {
        InputManager.Instance.OnStartTouch += DropShape;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnStartTouch -= DropShape;
    }


    Vector2 GetWorldCoordinates(Vector2 screenPos)
    {
        Vector2 screenCoord = new Vector2(screenPos.x, screenPos.y);
        Vector2 worldCoord = mainCam.ScreenToWorldPoint(screenCoord);
        return worldCoord;
    }

    void DropShape(Vector2 screenPosition)
    {
        int randomNumber = UnityEngine.Random.Range(0, listOfShapes.Count-1);

        float xValue = GetWorldCoordinates(screenPosition).x;
        if (xValue < -3.5f)
            xValue = -3.5f;
        else if (xValue > 3.5f)
            xValue = 3.5f;

        dropped++;
        dataText.text = $"Score: {score} \nDropped: {dropped}";
        StartCoroutine(GenerateShape(randomNumber, new Vector2(xValue, 3.95f)));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int randomNumber = UnityEngine.Random.Range(0, listOfShapes.Count - 1);
            StartCoroutine(GenerateShape(randomNumber, new Vector2(UnityEngine.Random.Range(-7.25f, 7.25f), 4.5f)));
        }
    }

    public IEnumerator GenerateShape(int num, Vector2 spawn)
    {
        yield return new WaitForSeconds(0.05f);

        try
        {
            Shape newShape = Instantiate(listOfShapes[num]);
            newShape.transform.position = spawn;
            newShape.Setup(num);
        }
        catch (ArgumentOutOfRangeException)
        {
            //do nothing
        }
    }

    public void AddScore(int num)
    {
        score += num;
        dataText.text = $"Score: {score} \nDropped: {dropped}";
    }

    public void GameOver()
    {
        Debug.Log("you lost");
    }
}
