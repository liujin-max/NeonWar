using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UrlImageUtility : MonoBehaviour
{
    private RawImage m_Image;

    void Awake()
    {
        m_Image = transform.GetComponent<RawImage>();
    }

    public void SetLocalImage(string url)
    {
        m_Image.texture = Resources.Load<Texture2D>(url);
        m_Image.SetNativeSize();
    }

    public void SetImage(string url)
    {
        if (string.IsNullOrEmpty(url)) return;

        StartCoroutine(LoadImage(url));
    }


    IEnumerator LoadImage(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // 将加载的纹理应用到你想要显示的对象上，比如RawImage组件
                m_Image.texture = DownloadHandlerTexture.GetContent(www);;
                m_Image.SetNativeSize();
            }
        }
    }
}
