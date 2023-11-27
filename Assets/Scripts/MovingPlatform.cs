using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] float travelTime;
    [SerializeField] float pauseTime;
    Vector2 originalLocation;
    [SerializeField] Vector2 newLocation;
    WaitForSeconds pauseEnumerator;

    private void Start()
    {
        originalLocation = this.transform.position;
        pauseEnumerator = new WaitForSeconds(pauseTime);
        StartCoroutine(MoveForwards());
    }

    IEnumerator MoveForwards()
    {
        float elapsedTime = 0f;

        while (elapsedTime < travelTime)
        {
            transform.position = Vector3.Lerp(originalLocation, newLocation, elapsedTime / travelTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return pauseEnumerator;
        StartCoroutine(MoveBackwards());
    }

    IEnumerator MoveBackwards()
    {
        float elapsedTime = 0f;

        while (elapsedTime < travelTime)
        {
            transform.position = Vector3.Lerp(newLocation, originalLocation, elapsedTime / travelTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return pauseEnumerator;
        StartCoroutine(MoveForwards());
    }
}
