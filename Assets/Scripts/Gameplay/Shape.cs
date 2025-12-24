using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;

public enum KindOfShape { None, Circle = 1, Square, Arrow, Diamond, Star, Hexagon, Heart, Crown, Bomb, Wall, Inversion, Warp }

[RequireComponent(typeof(Rigidbody2D))]
public class Shape : MonoBehaviour
{
    public SpriteRenderer spriterenderer;
    [ReadOnly] public Rigidbody2D rb;

    int value;
    [SerializeField] TMP_Text textBox;

    bool active = false;
    float deathLineTouched = 0f;
    public KindOfShape myShape;
    Vector3 mySize;

    private void Awake()
    {
        mySize = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        rb.angularDamping = 2;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        if (IsMainShape())
        {
            value = (int)Mathf.Pow((int)myShape, 2);
            textBox.text = $"{value}";
        }
    }

    public bool IsMainShape()
    {
        return (int)myShape <= (int)KindOfShape.Crown;
    }

    public void Setup(Vector3 start, float gravity)
    {
        active = false;
        this.transform.position = start;
        this.transform.localEulerAngles = Vector3.zero;
        this.transform.localScale = Vector3.zero;
        rb.gravityScale = gravity;
        this.gameObject.SetActive(true);
        rb.WakeUp();
        
        StartCoroutine(BecomeActive());
        IEnumerator BecomeActive()
        {
            float elapsedTime = 0f;
            float totalTime = 3/10f;
            while (elapsedTime < totalTime)
            {
                elapsedTime += Time.deltaTime;
                this.transform.localScale = Vector3.Lerp(Vector3.zero, mySize, elapsedTime/totalTime);
                yield return null;
            }
            active = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!active)
        {       
        }   
        else if (collision.TryGetComponent(out Shape otherShape) && otherShape.active)
        {
            if (this.IsMainShape())
            {
                if (otherShape.myShape == this.myShape && (rb.gravityScale > 0 && this.transform.position.y > otherShape.transform.position.y || rb.gravityScale < 0 && this.transform.position.y < otherShape.transform.position.y))
                {
                    active = false;
                    otherShape.active = false;
                    ChangeMe(otherShape, 1);
                }
            }
            if (otherShape.myShape == KindOfShape.Inversion)
            {
                otherShape.active = false;
                ShapeManager.instance.ReturnShape(otherShape);
                ShapeManager.instance.SwitchGravity();
            }
            if (otherShape.myShape == KindOfShape.Warp)
            {
                active = false;
                otherShape.active = false;

                float newXPosition = otherShape.transform.position.x > 0 ? ShapeManager.instance.leftWall.transform.position.x + 1f : ShapeManager.instance.rightWall.transform.position.x - 1f;
                ShapeManager.instance.GenerateShape(this.myShape, new(newXPosition, this.transform.position.y, 0));
                ShapeManager.instance.ReturnShape(otherShape);
                ShapeManager.instance.ReturnShape(this);
            }
            if (otherShape.myShape == KindOfShape.Bomb)
            {
                active = false;
                otherShape.active = false;

                AudioManager.instance.PlaySound(ShapeManager.instance.bombSound, 0.35f);
                ShapeManager.instance.ReturnShape(otherShape);
                ShapeManager.instance.ReturnShape(this);
            }
        }   
        else
        {
            if (collision.CompareTag("Out of Bounds"))
            {
                Debug.Log("went out of bounds");
                ShapeManager.instance.ReturnShape(this);
            }
            else if (IsMainShape() && collision.CompareTag("Death Line"))
            {
                if (deathLineTouched < 3f)
                {
                    deathLineTouched += Time.deltaTime;
                }
                else
                {
                    active = false;
                    ShapeManager.instance.GameOver(ToTranslate.You_Lost);
                }
            }
        }
    }

    void ChangeMe(Shape otherShape, int change)
    {
        if (otherShape.myShape == this.myShape)
            ShapeManager.instance.AddScore(value, this.transform.position, spriterenderer.color);
                
        if (this.myShape == KindOfShape.Crown)
        {
            ShapeManager.instance.mergedCrowns = true;
            if (PrefManager.GetSetting() == Setting.MergeCrown)
                ShapeManager.instance.GameOver(ToTranslate.You_Won);
        }
        else
        {
            KindOfShape nextShape = (KindOfShape)((int)myShape + change);
            ShapeManager.instance.GenerateShape(nextShape, this.transform.position);
        }
        ShapeManager.instance.ReturnShape(otherShape);
        ShapeManager.instance.ReturnShape(this);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Death Line"))
            deathLineTouched = 0f;
    }
}