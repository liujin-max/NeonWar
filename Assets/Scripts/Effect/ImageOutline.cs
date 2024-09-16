using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageOutline : MonoBehaviour
{
    private Image m_Sprite;
    public Material m_MatTemplate;

    private int m_ColorID;

    void Awake()
    {
        m_Sprite= GetComponent<Image>();
        m_Sprite.material = new Material(m_MatTemplate);


        m_ColorID   = Shader.PropertyToID("_OutlineColor");
    }

    public void SetColor(string color_string)
    {
        if (ColorUtility.TryParseHtmlString(color_string, out Color color)) {
            m_Sprite.material.SetColor(m_ColorID, color);
        }
    }
}
