using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Inverter : Shape
{
    [SerializeField] AudioClip gravitySound;
    public override Vector2 UISize(bool larger)
    {
        return larger ? new(90, 90) : new(60, 60);
    }
    protected override void HitOtherShape(Shape otherShape)
    {
        AudioManager.instance.PlaySound(gravitySound, 0.5f);
        ShapeManager.instance.SwitchGravity();
        ShapeManager.instance.StartCoroutine(ArrowAnimation());
        ShapeManager.instance.ReturnShape(this);
    }
    IEnumerator ArrowAnimation()
    {
        Transform gravityArrow = ShapeManager.instance.GetGravityArrow();

        Vector2 zeroSize = new(0, 0);
        Vector2 maxSize = new(3, 3);

        Vector3 currRot = gravityArrow.localEulerAngles;
        Vector3 newRot = new(0, 0, Mathf.Sign(Physics2D.gravity.y)*90);

        gravityArrow.localScale = zeroSize;

        float elapsedTime = 0f;
        float waitTime = 0.5f;
        while (elapsedTime < waitTime)
        {
            gravityArrow.localScale = Vector3.Lerp(zeroSize, maxSize, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gravityArrow.localScale = maxSize;

        elapsedTime = 0f;
        while (elapsedTime < waitTime)
        {
            gravityArrow.localEulerAngles = Vector3.Lerp(currRot, newRot, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gravityArrow.localEulerAngles = newRot;

        elapsedTime = 0f;
        while (elapsedTime < waitTime)
        {
            gravityArrow.localScale = Vector3.Lerp(maxSize, zeroSize, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gravityArrow.localScale = zeroSize;
    }
}
