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
    bool active = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.useAutoMass = true;
        rb.gravityScale = 2;
        rb.angularDrag = 10;

        try
        {
            textBox = transform.GetChild(0).GetComponent<TMP_Text>();
        }
        catch (UnityException)
        {
            //do nothing
        }
    }

    public void Bomb()
    {
        this.name = "Bomb";
        StartCoroutine(BecomeActive());
    }

    public void Setup(int num)
    {
        value = num;
        this.name = $"{value+1}";
        int score = (int)Mathf.Pow(value+1, 2);
        textBox.text = $"{score}";
        StartCoroutine(BecomeActive());
    }

    IEnumerator BecomeActive()
    {
        yield return new WaitForSeconds(0.25f);
        active = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (active && this.name == collision.name && this.transform.position.y > collision.transform.position.y)
        {
            active = false;
            StartCoroutine(Merge(collision));
        }

        if (active && collision.name == "Bomb")
        {
            active = false;
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }

        if (active && collision.CompareTag("Death Line"))
        {
            active = false;
            ShapeManager.instance.GameOver();
        }
    }

    IEnumerator Merge(Collider2D collision)
    {
        yield return ShapeManager.instance.GenerateShape(ShapeManager.instance.listOfShapes[value+1], this.transform.position);
        ShapeManager.instance.AddScore(int.Parse(textBox.text));

        if (collision != null)
            Destroy(collision.gameObject);
        Destroy(this.gameObject);
    }
}
