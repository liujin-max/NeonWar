using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Affected : MonoBehaviour
{
    private SpriteRenderer m_Sprite;
    private Material m_Mat = null;


    private bool m_IsDoing = false;
    private float m_Value;

    void Awake()
    {
        m_Sprite = GetComponent<SpriteRenderer>();
    }

    public void DoAnimation()
    {
        // if (m_IsDoing) return;

        if (m_Mat == null) {
            m_Mat   = Resources.Load<Material>("Meterial/Enemy");
            m_Sprite.material   = m_Mat;
        }

        StartCoroutine(InterpolateValue());
    }

    // Coroutine 来处理插值过程
    private IEnumerator InterpolateValue()
    {
        m_IsDoing   = true;
        m_Value     = 0;

        m_Sprite.transform.DOPunchPosition(new Vector3(0.04f, 0.04f, 0f), 0.15f, 50);


        float t1    = 0;
        float time1 = 0.1f;
        while (m_Value < 1f)
        {
            t1 += Time.deltaTime;
            m_Value = Mathf.Lerp(0f, 1f, t1 / time1);

            yield return null;
        }

        m_Value = 1f;
        yield return null;

        float t2    = 0;
        float time2 = 0.05f;
        while (m_Value > 0f)
        {
            t2 += Time.deltaTime;
            m_Value = Mathf.Lerp(1f, 0f, t2 / time2);

            yield return null;
        }

        m_IsDoing   = false;
        m_Value     = 0f; // 确保最终值为 0
    }

    void Update()
    {
        if (!m_IsDoing) return;

        m_Mat.SetFloat("_ChromAberrAmount", m_Value);
    }
}
