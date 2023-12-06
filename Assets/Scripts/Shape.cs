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

    [ReadOnly] public int value;
    public TMP_Text textBox;

    bool active = false;
    float deathLineTouched = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.useAutoMass = false;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void AltShape(float gravity)
    {
        this.name = this.name.Replace("(Clone)", "");
        rb.gravityScale = gravity;
        rb.mass = 2;
        rb.angularDrag = 2;
        StartCoroutine(BecomeActive());
    }

    public void Setup(int num, float gravity)
    {
        value = num;
        this.name = $"{value+1}";
        int score = (int)Mathf.Pow(value+1, 2);
        rb.mass = 2;
        textBox.text = $"{score}";
        rb.gravityScale = gravity;
        rb.angularDrag = 2;
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
            if (this.textBox != null && this.name == collision.name && this.transform.position.y > collision.transform.position.y)
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

            else if (collision.CompareTag("Out of Bounds"))
            {
                Destroy(this.gameObject);
            }

            else if (collision.CompareTag("Inversion"))
            {
                Destroy(collision.gameObject);
                ShapeManager.instance.SwitchGravity();
            }

            else if (textBox != null && collision.CompareTag("Death Line"))
            {
                if (deathLineTouched < 3f)
                {
                    deathLineTouched += Time.deltaTime;
                }
                else
                {
                    active = false;
                    ShapeManager.instance.GameOver("You Lost.", false);
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
        if (textBox != null)
        {
            ShapeManager.instance.AddScore(int.Parse(textBox.text), this);

            if (value + 1 >= ShapeManager.instance.listOfShapes.Count)
            {
                if (LevelSettings.instance.setting == TitleScreen.Setting.MergeCrown)
                    ShapeManager.instance.GameOver("You Won!", true);
            }
            else
            {
                yield return ShapeManager.instance.GenerateShape(ShapeManager.instance.listOfShapes[value + 1], this.transform.position);
            }

            if (collision != null)
                Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }
}