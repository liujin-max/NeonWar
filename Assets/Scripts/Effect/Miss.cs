using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Miss : MonoBehaviour
{

    [SerializeField] private TextMeshPro m_Text;
    [SerializeField] private TextMeshPro m_Text2;
    [SerializeField] private TextMeshPro m_Text3;
    [SerializeField] private TextMeshPro m_Text4;

    private List<Tweener> m_ShakeTweeners = new List<Tweener>();

    void OnEnable()
    {
        Shake();
    }

    void Shake()
    {
        m_Text.transform.localPosition  = Vector3.zero;
        m_Text2.transform.localPosition = Vector3.zero;
        m_Text3.transform.localPosition = Vector3.zero;
        m_Text4.transform.localPosition = Vector3.zero;

        if (m_ShakeTweeners.Count > 0)
        {
            m_ShakeTweeners.ForEach(t => t.Restart());
            return;
        }

        m_ShakeTweeners.Add(m_Text.transform.DOShakePosition(0.3f,  new Vector3(0.1f, 0.1f, 0), 50, 50).SetAutoKill(false));
        m_ShakeTweeners.Add(m_Text2.transform.DOShakePosition(0.6f, new Vector3(0.2f, 0.15f, 0), 50, 50).SetAutoKill(false));
        m_ShakeTweeners.Add(m_Text3.transform.DOShakePosition(0.6f, new Vector3(0.2f, 0.15f, 0), 50, 50).SetAutoKill(false));
        m_ShakeTweeners.Add(m_Text4.transform.DOShakePosition(0.6f, new Vector3(0.2f, 0.15f, 0), 50, 50).SetAutoKill(false));

    }
}
