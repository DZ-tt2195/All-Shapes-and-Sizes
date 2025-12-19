using System.Collections;
using UnityEngine;
using MyBox;
using TMPro;

public enum KindOfShape { None, Circle = 1, Square, Arrow, Diamond, Star, Hexagon, Heart, Crown, Bomb, Wall, Inversion, Upgrader }

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.angularDamping = 2;
        rb.useAutoMass = false;
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
        rb.gravityScale = gravity;
        this.gameObject.SetActive(true);
        Invoke(nameof(BecomeActive), 0.2f);
    }

    void BecomeActive()
    {
        active = true;
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
                if ((otherShape.myShape == this.myShape && this.transform.position.y > otherShape.transform.position.y) 
                || (otherShape.myShape == KindOfShape.Upgrader && this.myShape != KindOfShape.Crown))
                {
                    active = false;
                    otherShape.active = false;
                    StartCoroutine(Upgrade(otherShape));
                }
            }
            if (otherShape.myShape == KindOfShape.Inversion)
            {
                ShapeManager.instance.ReturnShape(otherShape);
                ShapeManager.instance.SwitchGravity();
            }
            if (otherShape.myShape == KindOfShape.Bomb)
            {
                active = false;
                AudioManager.instance.PlaySound(ShapeManager.instance.bombSound, 0.35f);
                ShapeManager.instance.ReturnShape(otherShape);
                ShapeManager.instance.ReturnShape(this);
            }
        }   
        else
        {
            if (collision.CompareTag("Out of Bounds"))
            {
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

    IEnumerator Upgrade(Shape otherShape)
    {
        yield return new WaitForSeconds(0.1f);

        ShapeManager.instance.AddScore(value, this.transform.position, spriterenderer.color);
        AudioManager.instance.PlaySound(ShapeManager.instance.scoreSound, 0.25f);
                
        if (this.myShape == KindOfShape.Crown)
        {
            ShapeManager.instance.mergedCrowns = true;
            if (PrefManager.GetSetting() == Setting.MergeCrown)
                ShapeManager.instance.GameOver(ToTranslate.You_Won);
        }
        else
        {
            KindOfShape nextShape = (KindOfShape)((int)myShape + 1);
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