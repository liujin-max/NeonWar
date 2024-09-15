using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class UIController<T, U> where T : UIController<T, U>, new() where U : BaseWindow
{
    protected U m_Window;

    
    private static T m_Instance;
    public static T Instance {
        get {
            if (m_Instance == null) m_Instance = new T();
            return m_Instance;
        }
    }

    public void Enter(Action<U> action = null)
    {
        OpenWindow(action);
        AddHandlers();
    }

    public void Exit()
    {
        CloseWindow();
        DelHandlers();
    }
    

    //打开Window
    protected abstract void OpenWindow(Action<U> action = null);

    //关闭Window
    protected virtual void CloseWindow()
    {
        if (m_Window != null) m_Window.Dispose();
    }

    //注册事件监听
    protected abstract void AddHandlers();
    //移除事件监听
    protected abstract void DelHandlers();
}
