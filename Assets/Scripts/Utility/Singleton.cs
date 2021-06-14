using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance)
        {
            Debug.LogError("[Singleton] Trying to instantiate a second instance of a singleton class");
        }
        else
        {
            Instance = (T)this;
        }
    }

    protected void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        OtherOnDestroyActions();
    }

    protected virtual void OtherOnDestroyActions() { }
}