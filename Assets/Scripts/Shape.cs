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
        rb.angularDrag = 10;

        textBox = transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void Setup(int num)
    {
        this.name = $"{num}";
        value = num;
        textBox.text = $"{(num+1)*2}";
    }

    private void Update()
    {
        if (this.transform.position.y > 4f)
            ShapeManager.instance.GameOver();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active && this.name == collision.name && this.transform.position.y <= collision.transform.position.y)
        {
            active = false;
            StartCoroutine(Merge(collision));
        }
    }

    IEnumerator Merge(Collider2D collision)
    {
        yield return ShapeManager.instance.GenerateShape(value + 1, this.transform.position);
        ShapeManager.instance.AddScore(int.Parse(textBox.text));

        if (collision != null)
            Destroy(collision.gameObject);
        Destroy(this.gameObject);
    }
}
