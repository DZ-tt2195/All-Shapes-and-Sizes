using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class Shape : MonoBehaviour
{
    public SpriteRenderer spriterenderer;
    protected Rigidbody2D rb;
    [SerializeField] protected int value;
    protected TMP_Text textBox;
    [ReadOnly] public bool canInteract;
    float deathLineTouched = 0f;
    Vector3 mySize;
    private void Awake()
    {
        mySize = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 2.75f;
        rb.angularDamping = 2;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        if (IsMainShape())
        {
            textBox = this.transform.GetComponentInChildren<TMP_Text>();
            value = (int)Mathf.Pow(value, 2);
            textBox.text = $"{value}";
        }
    }
    public virtual Vector2 UISize(bool larger)
    {
        return Vector2.zero;
    }
    public bool IsMainShape() => value >= 1;
    public virtual void Setup(Vector2 start)
    {
        canInteract = false;
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
                this.transform.localScale = Vector3.Lerp(Vector3.zero, mySize, elapsedTime/totalTime);
                yield return null;
            }
            canInteract = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!canInteract) return;
        if (collision.TryGetComponent(out Shape otherShape) && otherShape.canInteract)
        {
            if (this.IsMainShape() && otherShape.IsMainShape() && this.transform.position.y > otherShape.transform.position.y) 
                return;
            HitOtherShape(otherShape);
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
                    canInteract = false;
                    ShapeManager.instance.GameOver(AutoTranslate.Game_Over());
                }
            }
        }
    }
    protected virtual void HitOtherShape(Shape otherShape)
    {
    }
    protected void ScoreShapes(Shape otherShape)
    {
        ShapeManager.instance.AddScore(value, this.transform.position, spriterenderer.color);
        ShapeManager.instance.ReturnShape(otherShape);
        ShapeManager.instance.ReturnShape(this);        
    }
    protected virtual void Upgrade(Shape otherShape)
    {
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Death Line"))
            deathLineTouched = 0f;
    }
    void Update()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, ShapeManager.instance.leftWall.position.x, ShapeManager.instance.rightWall.position.x);
        if (Physics2D.gravity.y < 0)
            pos.y = Mathf.Max(pos.y, ShapeManager.instance.floor.position.y);
        else if (Physics2D.gravity.y > 0)
            pos.y = Mathf.Min(pos.y, ShapeManager.instance.ceiling.position.y);

        transform.position = pos;
    }
}