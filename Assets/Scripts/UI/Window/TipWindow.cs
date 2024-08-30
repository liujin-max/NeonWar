using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TipWindow : MonoBehaviour
{
    [SerializeField] private GameObject m_TipPivot;


    private Sequence m_Sequence;

    void Awake()
    {
        m_TipPivot.SetActive(false);
        
        EventManager.AddHandler(EVENT.UI_POPUPTIP,  OnTip);
    }

    void OnDestroy()
    {
        EventManager.AddHandler(EVENT.UI_POPUPTIP,  OnTip);
    }

    private void OnTip(GameEvent @event)
    {
        Platform.Instance.VIBRATE(_C.VIBRATELEVEL.MEDIUM);
        SoundManager.Instance.Load(SOUND.TIP);
 
        var text        = (string)@event.GetParam(0);

        m_TipPivot.SetActive(true);
        m_TipPivot.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
        

        if (m_Sequence != null) {
            m_Sequence.Kill();
            m_TipPivot.transform.localPosition = new Vector3(0, 350, 0);
        }
        var group   = m_TipPivot.GetComponent<CanvasGroup>();
        group.alpha = 1;

        m_Sequence = DOTween.Sequence();
        m_Sequence.Join(m_TipPivot.transform.DOShakePosition(0.25f, new Vector3(10, 0, 0), 40, 50));
        m_Sequence.Join(group.DOFade(1f, 1.5f));
        m_Sequence.Append(group.DOFade(0f, 0.5f));
        m_Sequence.AppendCallback(()=>{
            m_TipPivot.SetActive(false);
        });
        m_Sequence.Play();
    }
}
