using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private Effect m_Effect;

    void Awake()
    {
        m_Effect = GetComponent<Effect>();
    }

    public void Show()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f);
    }

    public void Dispose()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1.4f, 0.15f));
        seq.Append(transform.DOScale(0, 0.4f));
        seq.AppendCallback(()=>{
            m_Effect.Dispose();
        });
        seq.Play();
    }
}
