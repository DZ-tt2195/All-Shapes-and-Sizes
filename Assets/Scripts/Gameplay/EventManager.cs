using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
#region Setup
    public static EventManager inst;
    Dictionary<Type, List<Delegate>> triggers = new();
    Dictionary<Type, List<Delegate>> modifiers = new();
    void Awake()
    {
        inst = this;        
    }
    #endregion
#region Triggers
    public void Subscribe<T>(Action<T> function) where T : CustomEvent
    {
        Type type = typeof(T);
        if (!triggers.ContainsKey(type)) triggers.Add(type, new List<Delegate>());
        triggers[type].Add(function);
    }
    public void Unsubscribe<T>(Action<T> function) where T : CustomEvent
    {
        Type type = typeof(T);
        triggers[type].Remove(function);
    }
    public void RunTriggers<T>(T info) where T : CustomEvent
    {
        Type type = typeof(T);
        if (triggers.ContainsKey(type))
        {
            List<Delegate> copiedList = new(triggers[type]);
            foreach (Delegate function in copiedList)
                ((Action<T>)function).Invoke(info);
        }
    }
#endregion
#region Modifiers
    public void Subscribe<T, TResult>(Func<T, TResult> function) where T : CustomEvent<TResult>
    {
        Type type = typeof(T);
        if (!modifiers.ContainsKey(type)) modifiers.Add(type, new());
        modifiers[type].Add(function);
    }
    public void Unsubscribe<T, TResult>(Func<T, TResult> function) where T : CustomEvent<TResult>
    {
        Type type = typeof(T);
        triggers[type].Remove(function);
    }
    public List<TResult> GetModifiers<T, TResult>(T info) where T : CustomEvent<TResult>
    {
        Type type = typeof(T);
        List<TResult> allModifiers = new();
        if (triggers.ContainsKey(type))
        {
            List<Delegate> copiedList = new(triggers[type]);
            foreach (Delegate function in copiedList)
                allModifiers.Add(((Func<T, TResult>)function).Invoke(info));
        }
        return allModifiers;
    }
#endregion
}
public abstract class CustomEvent
{
}
public abstract class CustomEvent<TResult> : CustomEvent
{
}
/*
public class EventDeath : CustomEvent
{
    public GameObject deadObject { get; private set; }

    public EventDeath(GameObject gone)
    {
        deadObject = gone;
    }
}
Subscribe<EventDeath>(death =>
{
    Debug.Log(death.deadObject.name);
});
RunTriggers(new EventDeath(this.gameObject));

        EventManager.inst.Subscribe<QuickTest, int>(Trying);

        int Trying(QuickTest test)
        {
            return 0;
        }
public class QuickTest : CustomEvent<int>
{
    public int testInt {get; private set;}

    public QuickTest(int test)
    {
        this.testInt = test;
    }
}
*/