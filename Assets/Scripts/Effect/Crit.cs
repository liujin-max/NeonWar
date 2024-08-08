using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Crit : MonoBehaviour
{

    [SerializeField] private TextMeshPro m_Text;
    [SerializeField] private TextMeshPro m_Text2;
    [SerializeField] private TextMeshPro m_Text3;
    [SerializeField] private TextMeshPro m_Text4;


    void OnEnable()
    {
        Shake();
    }

    void Shake()
    {
        GetComponent<Animation>().Play("CritShow");

        m_Text.transform.localPosition  = Vector3.zero;
        m_Text2.transform.localPosition = Vector3.zero;
        m_Text3.transform.localPosition = Vector3.zero;
        m_Text4.transform.localPosition = Vector3.zero;

        m_Text.transform.DOShakePosition(0.3f,  new Vector3(0.1f, 0.1f, 0), 50, 50);
        m_Text2.transform.DOShakePosition(0.6f, new Vector3(0.2f, 0.2f, 0), 50, 50);
        m_Text3.transform.DOShakePosition(0.6f, new Vector3(0.2f, 0.2f, 0), 50, 50);
        m_Text4.transform.DOShakePosition(0.6f, new Vector3(0.2f, 0.2f, 0), 50, 50);

    }
}
