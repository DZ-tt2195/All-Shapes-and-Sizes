using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Spinner : MonoBehaviour
{
    [Tooltip(">0 to go right, <0 to go left")] public float rotationSpeed;

    private void Update()
    {
        this.transform.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);
    }
}