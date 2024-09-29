using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconAnim : MonoBehaviour
{
    public bool AutoStart = true;

    
    void Start()
    {
        if (AutoStart) Play();
    }

    public virtual void Play()
    {

    }

    public virtual void Stop()
    {

    }
}
