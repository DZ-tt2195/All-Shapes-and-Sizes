using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;
[RequireComponent(typeof(Rigidbody2D))]
public class Shape : MonoBehaviour
{
#region Setup
    [SerializeField] SpriteRenderer spriterenderer; public SpriteRenderer GetSpriteRenderer => spriterenderer;
    [SerializeField] int dropChance; public int DropChance => dropChance;
    Rigidbody2D rb; public Rigidbody2D GetRB => rb;
    [SerializeField] protected int value;
    [SerializeField] protected TMP_Text textBox;
    [ReadOnly] public bool canInteract;
    float deathLineTouched = 0f;
    public bool cursed {get; private set;}
    Color originalShapeColor;
    Color originalFontColor;
    Vector3 originalSize;
    protected Dictionary<GameObject, Shape> shapesTouchingThis = new();
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 2.75f;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        rb.angularDamping = 2;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        originalSize = transform.localScale;
        originalShapeColor = spriterenderer.color;
        if (textBox != null) originalFontColor = textBox.color;
        if (IsMainShape()) 
        {
            value = (int)Mathf.Pow(value, 2);
            if (textBox != null) textBox.text = value.ToString();
        }
    }
    public virtual Vector2 UISize(bool larger) => new Vector2(50, 50);
    public bool IsMainShape() => value >= 1;
    public void PopUp(Vector2 start, bool cursed, bool doSetup)
    {
        CursedStatus(cursed);
        canInteract = false;
        rb.constraints = RigidbodyConstraints2D.None;
        shapesTouchingThis = new();
        this.transform.position = start;
        this.transform.localEulerAngles = Vector3.zero;
        this.transform.localScale = Vector3.zero;        
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
                this.transform.localScale = Vector3.Lerp(Vector3.zero, originalSize, elapsedTime/totalTime);
                yield return null;
            }
            canInteract = true;
        }

        if (doSetup)
            Setup();
    }
    protected virtual void Setup()
    {
    }
    public virtual string MyText() => Translator.inst.Translate(this.name);
#endregion
#region Gameplay
    public void CursedStatus(bool cursed)
    {
        if (cursed == true && !this.IsMainShape()) return;
        this.cursed = cursed;
        this.spriterenderer.color = cursed ? Color.black : originalShapeColor;
        if (textBox != null)
            textBox.color = cursed ? Color.white : originalFontColor;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!shapesTouchingThis.ContainsKey(collision.gameObject) && collision.TryGetComponent(out Shape shape))
            shapesTouchingThis.Add(collision.gameObject, shape);
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (!this.canInteract) return;
        if (shapesTouchingThis.ContainsKey(collision.gameObject))
        {
            Shape otherShape = shapesTouchingThis[collision.gameObject];
            if (!otherShape.canInteract) return;
            if (this.cursed && otherShape.cursed) return;
            HitOtherShape(otherShape);
        }
        else
        {
            if (collision.CompareTag("Out of Bounds"))
            {
                Debug.Log($"{this.name} went out of bounds");
                ShapeManager.inst.ReturnShape(this, ReturnType.Done);
            }
            else if (IsMainShape() && collision.CompareTag("Death Line"))
            {
                if (deathLineTouched < 5f)
                {
                    deathLineTouched += Time.deltaTime;
                }
                else
                {
                    canInteract = false;
                    ShapeManager.inst.GameOver(AutoTranslate.Game_Over());
                }
            }
        }
    }
    protected virtual void HitOtherShape(Shape otherShape)
    {
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (shapesTouchingThis.ContainsKey(collision.gameObject))
            shapesTouchingThis.Remove(collision.gameObject);
        else if (collision.CompareTag("Death Line"))
            deathLineTouched = 0f;
    }
    public void ScoreShapes(Shape otherShape, string newShape, bool cursed = false)
    {
        ShapeManager.inst.AddScore(value, this.transform.position, spriterenderer.color);
        if (otherShape != null)
        {
            CreateNext(Vector2.Lerp(this.transform.position, otherShape.transform.position, 0.5f));
            ShapeManager.inst.ReturnShape(otherShape, ReturnType.Combine);
        }
        else
        {
            CreateNext(this.transform.position);
        }
        void CreateNext(Vector2 spawn)
        {
            if (newShape != "")
                ShapeManager.inst.GenerateShape(newShape, spawn, CreationType.Combine, cursed);
        }
        ShapeManager.inst.ReturnShape(this, ReturnType.Combine);
    }
    public virtual void OnReturn(ReturnType returnType)
    {
    }
#endregion
}