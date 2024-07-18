using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Button m_button;
    private Tweener m_Tweener = null;

    void Awake()
    {
        m_button = transform.GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SoundManager.Instance.Load(SOUND.CLICK);
        Platform.Instance.VIBRATE(_C.VIBRATELEVEL.LIGHT);

        if (m_Tweener != null) {
            m_Tweener.Kill();
        }

        m_Tweener = m_button.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_Tweener != null) {
            m_Tweener.Kill();
        }

        m_Tweener = m_button.transform.DOScale(Vector3.one, 0.1f);
    }
}
