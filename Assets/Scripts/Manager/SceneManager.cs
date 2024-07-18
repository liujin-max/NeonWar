using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScenePool: MonoBehaviour
{
     // 异步加载场景的方法
    public void LoadSceneAsync(string sceneName, Action listener = null)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, listener));
    }
    
    private IEnumerator LoadSceneCoroutine(string sceneName, Action listener = null)
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 获取加载进度（范围从0到1）
            // Debug.Log("Loading progress: " + (progress * 100) + "%");

            yield return null;
        }

        if (listener != null) {
            listener.Invoke();
        }
    }
}
