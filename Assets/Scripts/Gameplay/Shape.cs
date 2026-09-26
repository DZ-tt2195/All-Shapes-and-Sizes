using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;
[RequireComponent(typeof(Rigidbody2D))]
public class Shape : MonoBehaviour
{

#region Setup
    public SpriteRenderer spriterenderer;
    [SerializeField] int dropChance; public int DropChance => dropChance;
    Rigidbody2D rb;
    [SerializeField] protected int value;
    [SerializeField] protected TMP_Text textBox;
    [ReadOnly] public bool canInteract;
    float deathLineTouched = 0f;
    public bool cursed {get; private set;}
    Color originalShapeColor;
    Color originalFontColor;
    Vector3 originalSize;
    HashSet<GameObject> disableColliders = new();
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
        if (IsMainShape()) value = (int)Mathf.Pow(value, 2);
    }
    public virtual Vector2 UISize(bool larger) => new Vector2(50, 50);
    public bool IsMainShape() => value >= 1;
    public bool HasAbility() => disableColliders.Count == 0 && canInteract;
    public virtual void Setup(Vector2 start, bool cursed)
    {
        canInteract = false;
        shapesTouchingThis = new();
        this.transform.position = start;
        this.transform.localEulerAngles = Vector3.zero;
        this.transform.localScale = Vector3.zero;
        CursedStatus(cursed);
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
    }
#endregion

#region Gameplay
    public void CursedStatus(bool cursed)
    {
        if (cursed == true && !this.IsMainShape()) return;
        this.cursed = cursed;
        this.spriterenderer.color = cursed ? Color.black : originalShapeColor;
        if (textBox != null)
        {
            if (IsMainShape()) textBox.text = value.ToString();
            textBox.color = cursed ? Color.white : originalFontColor;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!shapesTouchingThis.ContainsKey(collision.gameObject) && collision.TryGetComponent(out Shape shape))
            shapesTouchingThis.Add(collision.gameObject, shape);
        else if (!disableColliders.Contains(collision.gameObject) && collision.CompareTag("Disable"))
            disableColliders.Add(collision.gameObject);
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (!this.HasAbility()) return;
        if (shapesTouchingThis.ContainsKey(collision.gameObject))
        {
            Shape otherShape = shapesTouchingThis[collision.gameObject];
            if (!otherShape.HasAbility()) return;
            if (this.IsMainShape() && otherShape.IsMainShape() && this.transform.position.y > otherShape.transform.position.y) return;
            if (this.cursed && otherShape.cursed) return;
            HitOtherShape(otherShape);
        }   
        else
        {
            if (collision.CompareTag("Out of Bounds"))
            {
                Debug.Log("went out of bounds");
                ShapeManager.inst.ReturnShape(this);
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
        else if (disableColliders.Contains(collision.gameObject))
            disableColliders.Remove(collision.gameObject);
    }
    public void ScoreShapes(Shape otherShape, string newShape, bool cursed = false)
    {
        ShapeManager.inst.AddScore(value, this.transform.position, spriterenderer.color);
        if (otherShape != null)
        {
            CreateShape(Vector2.Lerp(this.transform.position, otherShape.transform.position, 0.5f));
            ShapeManager.inst.ReturnShape(otherShape);
        }
        else
        {
            CreateShape(this.transform.position);
        }
        void CreateShape(Vector2 spawn)
        {
            if (newShape != "")
                ShapeManager.inst.GenerateShape(newShape, spawn, CreationType.Combine, cursed);
        }
        ShapeManager.inst.ReturnShape(this);
    }
    public virtual bool TrackNewShapes() => false;
    public virtual void OnNewShape(Shape newShape, CreationType creationType)
    {
    }
#endregion

}