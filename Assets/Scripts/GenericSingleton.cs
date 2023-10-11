using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSingleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    public virtual void Awake()
    {
        if(instance==null)
        {
            instance = this as T;
        }
        else
        {
            Destroy(this);
        }
    }
}
