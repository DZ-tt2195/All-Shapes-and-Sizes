using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class PointsVisual : MonoBehaviour
{
    [SerializeField] TMP_Text textbox;
    Vector2 zeroSize = new(0, 0);
    float duration;
    int value;

    private void Start()
    {
        transform.localScale = zeroSize;
    }

    public void Setup(string text, Vector3 position, float duration)
    {
        this.transform.localPosition = position + new Vector3(0, 0, -1);
        this.duration = duration;
        textbox.text = text;
        value = 3;
        StartCoroutine(ExpandContract());
    }

    public void Setup(int score, Shape shape, float duration)
    {
        this.transform.localPosition = shape.transform.localPosition + new Vector3(0, 0, -1);
        this.value = shape.value;
        this.duration = duration;
        textbox.text = $"+{score}";
        textbox.color = shape.spriterenderer.color;
        StartCoroutine(ExpandContract());
    }

    IEnumerator ExpandContract()
    {
        Vector2 maxSize = (value <= 2) ? new(2, 2): new(value, value);
        float elapsedTime = 0f;
        float waitTime = duration / 2;

        while (elapsedTime < waitTime)
        {
            transform.localScale = Vector3.Lerp(zeroSize, maxSize, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            transform.SetAsFirstSibling();
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < waitTime)
        {
            transform.localScale = Vector3.Lerp(maxSize, zeroSize, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            transform.SetAsFirstSibling();
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
