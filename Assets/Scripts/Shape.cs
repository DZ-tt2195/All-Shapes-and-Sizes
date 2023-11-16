using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class Shape : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(this.tag))
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }
}
