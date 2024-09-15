using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Button m_button;
    private Tweener m_DownTweener = null;
    private Tweener m_UpTweener;

    void Awake()
    {
        m_button = transform.GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SoundManager.Instance.Load(SOUND.CLICK);
        Platform.Instance.VIBRATE(CONST.VIBRATELEVEL.LIGHT);

        if (m_DownTweener != null) {
            m_DownTweener.Restart();
            return;
        }

        m_DownTweener = m_button.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f).SetAutoKill(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_UpTweener != null) {
            m_UpTweener.Restart();
            return;
        }

        m_UpTweener = m_button.transform.DOScale(Vector3.one, 0.1f).SetAutoKill(false);
    }
}
