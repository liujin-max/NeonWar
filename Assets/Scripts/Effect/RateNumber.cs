using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RateNumber : MonoBehaviour
{

    [SerializeField] private Text m_Text;
    [SerializeField] private Text m_Text2;
    [SerializeField] private Text m_Text3;
    [SerializeField] private Text m_Text4;

    public void Fly(float value)
    {
        m_Text.text = string.Format("x{0}", value);

        m_Text2.text = m_Text.text;
        m_Text3.text = m_Text.text;
        m_Text4.text = m_Text.text;

        // Platform.Instance.VIBRATE(_C.VIBRATELEVEL.LIGHT);
        Shake();
    }

    void Shake()
    {
        m_Text.transform.localPosition  = Vector3.zero;
        m_Text2.transform.localPosition = Vector3.zero;
        m_Text3.transform.localPosition = Vector3.zero;
        m_Text4.transform.localPosition = Vector3.zero;


        m_Text.transform.DOShakePosition(0.3f, new Vector3(20, 0, 0), 50, 50);
        m_Text.transform.DOLocalMoveY(m_Text.transform.transform.localPosition.y + 50, 0.6f);

        m_Text2.transform.DOShakePosition(0.6f, new Vector3(30, 5, 0), 50, 50);
        m_Text2.transform.DOLocalMoveY(m_Text2.transform.transform.localPosition.y + 50, 0.6f);

        m_Text3.transform.DOShakePosition(0.6f, new Vector3(30, 5, 0), 50, 50);
        m_Text3.transform.DOLocalMoveY(m_Text3.transform.transform.localPosition.y + 50, 0.6f);

        m_Text4.transform.DOShakePosition(0.6f, new Vector3(30, 5, 0), 50, 50);
        m_Text4.transform.DOLocalMoveY(m_Text4.transform.transform.localPosition.y + 50, 0.6f);
    }
}
