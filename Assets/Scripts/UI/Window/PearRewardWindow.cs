using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PearRewardWindow : BaseWindow
{
    [SerializeField] private Button m_Mask;
    [SerializeField] private Transform m_PearPivot;


    void Awake()
    {
        StartCoroutine(UIOpen());
    }

    public void Init(List<Pear> pears)
    {
        InitPears(pears);
    }

    void InitPears(List<Pear> pears)
    {
        foreach (var pear in pears)
        {
            var item = GameFacade.Instance.UIManager.LoadItem("PearItem", m_PearPivot).GetComponent<PearItem>();
            item.transform.localScale = new Vector3(1.6f, 1.6f, 1);
            item.Init(pear);
            item.ShowTagEquip(false);
        }
    }

    IEnumerator UIOpen()
    {
        yield return new WaitForSeconds(0.6f);

        m_Mask.onClick.AddListener(()=>{ UICtrl_PearRewardWindow.Instance.Exit();});

        yield return null;
    }
}
