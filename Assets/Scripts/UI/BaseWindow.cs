using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Window基类
public class BaseWindow : MonoBehaviour
{
    public virtual void Dispose()
    {
        GameFacade.Instance.UIManager.UnloadWindow(gameObject);
    }
}
