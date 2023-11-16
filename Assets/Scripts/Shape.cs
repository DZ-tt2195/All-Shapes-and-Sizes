using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class Shape : MonoBehaviour
{
    Rigidbody2D rb;
    int value;
    TMP_Text textBox;
    bool active = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.useAutoMass = true;
        rb.gravityScale = 2;

        textBox = transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void Setup(int num)
    {
        value = num;
        textBox.text = $"{(num+1)*2}";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active && collision.CompareTag(this.tag) && this.transform.position.y <= collision.transform.position.y)
        {
            active = false;
            StartCoroutine(Merge(collision));
        }
    }

    IEnumerator Merge(Collider2D collision)
    {
        yield return (Manager.instance.GenerateShape(value + 1, this.transform.position));
        if (collision.gameObject != null)
            Destroy(collision.gameObject);
        Destroy(this.gameObject);
    }
}
