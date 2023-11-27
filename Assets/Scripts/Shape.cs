using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Shape : MonoBehaviour
{
    public SpriteRenderer spriterenderer;
    [ReadOnly] public Rigidbody2D rb;
    int value;
    TMP_Text textBox;
    bool active = false;
    float deathLineTouched = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.useAutoMass = true;
        rb.angularDrag = 10;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        try
        {
            textBox = transform.GetChild(0).GetComponent<TMP_Text>();
        }
        catch (UnityException)
        {
            //do nothing
        }
    }

    public void AltShape(int gravity)
    {
        this.name = this.name.Replace("(Clone)", "");
        rb.gravityScale = gravity;
        StartCoroutine(BecomeActive());
    }

    public void Setup(int num, int gravity)
    {
        value = num;
        this.name = $"{value+1}";
        int score = (int)Mathf.Pow(value+1, 2);
        textBox.text = $"{score}";
        rb.gravityScale = gravity;
        StartCoroutine(BecomeActive());
    }

    IEnumerator BecomeActive()
    {
        yield return new WaitForSeconds(0.2f);
        active = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (active)
        {
            if (this.name == collision.name && this.transform.position.y > collision.transform.position.y)
            {
                Shape otherShape = collision.gameObject.GetComponent<Shape>();
                if (otherShape.active)
                {
                    active = false;
                    otherShape.active = false;
                    StartCoroutine(Merge(collision));
                }
            }

            else if (collision.CompareTag("Bomb"))
            {
                active = false;
                Destroy(collision.gameObject);
                Destroy(this.gameObject);
            }

            else if (collision.CompareTag("Inversion") && ShapeManager.instance.canPlay)
            {
                Destroy(collision.gameObject);
                ShapeManager.instance.SwitchGravity();
            }

            else if (collision.CompareTag("Death Line"))
            {
                if (deathLineTouched < 1.5f)
                {
                    deathLineTouched += Time.deltaTime;
                    Debug.Log($"{deathLineTouched:F2}");
                }
                else
                {
                    active = false;
                    ShapeManager.instance.GameOver("You Lost.");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Death Line"))
            deathLineTouched = 0f;
    }

    IEnumerator Merge(Collider2D collision)
    {
        if (value + 1 >= ShapeManager.instance.listOfShapes.Count)
        {
            ShapeManager.instance.GameOver("You Won!");
        }
        else
        {
            yield return ShapeManager.instance.GenerateShape(ShapeManager.instance.listOfShapes[value + 1], this.transform.position);
            ShapeManager.instance.AddScore(int.Parse(textBox.text));
        }

        if (collision != null)
            Destroy(collision.gameObject);
        Destroy(this.gameObject);
    }
}
