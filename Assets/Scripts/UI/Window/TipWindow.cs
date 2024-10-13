using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TipWindow : BaseWindow
{
    [SerializeField] private GameObject m_TipPivot;


    private Sequence m_Sequence;

    void Awake()
    {
        m_TipPivot.SetActive(false);
    }

    void OnEnable()
    {
        Event_PopupTip.OnEvent += OnTip;
    }

    void OnDisable()
    {
        Event_PopupTip.OnEvent -= OnTip;
    } 



    private void OnTip(Event_PopupTip e)
    {
        Platform.Instance.VIBRATE(VIBRATELEVEL.MEDIUM);

        if (!e.IsSilence) SoundManager.Instance.Load(SOUND.TIP);
 
        var text = e.Text;

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
