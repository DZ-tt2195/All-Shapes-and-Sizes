using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;
[RequireComponent(typeof(Rigidbody2D))]
public class Shape : MonoBehaviour
{
    public SpriteRenderer spriterenderer;
    Rigidbody2D rb;
    [SerializeField] protected int value;
    [SerializeField] protected TMP_Text textBox;
    [ReadOnly] public bool canInteract;
    float deathLineTouched = 0f;
    public bool cursed {get; private set;}
    Color originalShapeColor;
    Color originalFontColor;
    Vector3 originalSize;
    HashSet<GameObject> blackHoleColliders = new();
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 2.75f;
        rb.angularDamping = 2;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        originalSize = transform.localScale;
        originalShapeColor = spriterenderer.color;
        if (textBox != null)
            originalFontColor = textBox.color;

        if (IsMainShape())
            value = (int)Mathf.Pow(value, 2);
    }
    public virtual Vector2 UISize(bool larger) => Vector2.zero;
    public bool IsMainShape() => value >= 1;
    public bool HasAbility() => blackHoleColliders.Count == 0 && canInteract;
    public virtual void Setup(Vector2 start, bool cursed)
    {
        canInteract = false;
        this.transform.position = start;
        this.transform.localEulerAngles = Vector3.zero;
        this.transform.localScale = Vector3.zero;
        this.gameObject.SetActive(true);
        CursedStatus(cursed);
        rb.WakeUp();
        
        StartCoroutine(BecomeActive());
        IEnumerator BecomeActive()
        {
            float elapsedTime = 0f;
            float totalTime = 3/10f;
            while (elapsedTime < totalTime)
            {
                elapsedTime += Time.deltaTime;
                this.transform.localScale = Vector3.Lerp(Vector3.zero, originalSize, elapsedTime/totalTime);
                yield return null;
            }
            canInteract = true;
        }
    }
    public void CursedStatus(bool cursed)
    {
        this.cursed = cursed;
        this.spriterenderer.color = cursed ? Color.black : originalShapeColor;
        if (IsMainShape()) textBox.text = value.ToString();
        if (textBox != null) textBox.color = cursed ? Color.white : originalFontColor;
    }
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (!HasAbility()) return;
        if (collision.TryGetComponent(out Shape otherShape) && otherShape.HasAbility())
        {
            if (this.IsMainShape() && otherShape.IsMainShape() && this.transform.position.y > otherShape.transform.position.y) return;
            if (this.cursed && otherShape.cursed) return;
            HitOtherShape(otherShape);
        }   
        else
        {
            if (collision.CompareTag("Out of Bounds"))
            {
                Debug.Log("went out of bounds");
                ShapeManager.instance.ReturnShape(this);
            }
            else if (collision.CompareTag("BlackHole"))
            {
                blackHoleColliders.Add(collision.gameObject);
            }
            else if (IsMainShape() && collision.CompareTag("Death Line"))
            {
                if (deathLineTouched < 3f)
                {
                    deathLineTouched += Time.deltaTime;
                }
                else
                {
                    canInteract = false;
                    ShapeManager.instance.GameOver(AutoTranslate.Game_Over());
                }
            }
        }
    }
    protected virtual void HitOtherShape(Shape otherShape)
    {
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Death Line"))
            deathLineTouched = 0f;
        else if (collision.CompareTag("BlackHole"))
            blackHoleColliders.Remove(collision.gameObject);
    }
    public void ScoreShapes(Shape otherShape, string newShape)
    {
        ShapeManager.instance.AddScore(value, this.transform.position, spriterenderer.color);
        if (otherShape != null)
        {
            CreateShape(Vector2.Lerp(this.transform.position, otherShape.transform.position, 0.5f));
            ShapeManager.instance.ReturnShape(otherShape);
        }
        else
        {
            CreateShape(this.transform.position);
        }
        void CreateShape(Vector2 spawn)
        {
            if (newShape != "")
                ShapeManager.instance.GenerateShape(newShape, spawn, CreationType.Merge);
        }
        ShapeManager.instance.ReturnShape(this);
    }
}