using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public delegate void StartTouchEvent(Vector2 position);
    public event StartTouchEvent OnStartTouch;

    public delegate void EndTouchEvent(Vector2 position);
    public event EndTouchEvent OnEndTouch;

    public delegate void TouchMovedEvent(Vector2 position);
    public event TouchMovedEvent OnTouchMoved;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += FingerUp;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerMove += FingerMoved;
    }

    

    private void OnDisable()
    {
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= FingerUp;
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerMove -= FingerMoved;
        TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();
    }


    private void FingerDown(Finger finger)
    {

        if (OnStartTouch != null)
        {
            OnStartTouch(finger.screenPosition);
        }
    }

    private void FingerUp(Finger finger)
    {

        if (OnEndTouch != null)
        {
            OnEndTouch(finger.screenPosition);
        }
    }

    private void FingerMoved(Finger finger)
    {
        if(OnTouchMoved != null)
        {
            OnTouchMoved(finger.screenPosition);
        }
    }
}
