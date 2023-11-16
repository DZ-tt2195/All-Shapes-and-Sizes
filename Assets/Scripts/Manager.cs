using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;

public class Manager : MonoBehaviour
{
    public static Manager instance;
    [ReadOnly] public Camera mainCam;
    [SerializeField] List<Shape> listOfShapes = new List<Shape>();

    private void Awake()
    {
        instance = this;
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
        int randomNumber = UnityEngine.Random.Range(0, listOfShapes.Count);
        StartCoroutine(GenerateShape(randomNumber, new Vector2(GetWorldCoordinates(screenPosition).x, 4.5f)));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int randomNumber = UnityEngine.Random.Range(0, listOfShapes.Count);
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
}
