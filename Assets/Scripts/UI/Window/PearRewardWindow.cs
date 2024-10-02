using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PearRewardWindow : BaseWindow
{
    [SerializeField] private Button m_Mask;
    [SerializeField] private Image m_Icon;
    [SerializeField] private Transform m_DetailPivot;

    void Awake()
    {
        StartCoroutine(UIOpen());
    }

    public void Init(Pear pear)
    {
        InitPear(pear);
        InitDetail(pear);

        GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.COMPOSE, m_Icon.transform.position);
    }

    void InitPear(Pear pear)
    {
        string color_string = CONST.LEVEL_COLOR_PAIRS[pear.Level].Trim('<', '>');

        m_Icon.sprite = GameFacade.Instance.AssetManager.LoadSprite("Pear" , pear.ID.ToString());
        m_Icon.SetNativeSize();

        m_Icon.GetComponent<ImageOutline>().SetColor(color_string);
    }

    void InitDetail(Pear pear)
    {
        var item = GameFacade.Instance.UIManager.LoadItem("PearDetailItem", m_DetailPivot).GetComponent<PearDetailItem>();
        item.Init(pear, true);
    }

    IEnumerator UIOpen()
    {
        yield return new WaitForSeconds(0.6f);

        m_Mask.onClick.AddListener(()=>{ UICtrl_PearRewardWindow.Instance.Exit();});

        yield return null;
    }
}
