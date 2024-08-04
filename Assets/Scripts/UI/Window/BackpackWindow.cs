using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackWindow : MonoBehaviour
{
    [SerializeField] private Button m_Mask;
    [SerializeField] private SuperScrollView m_PearView;
    [SerializeField] private Transform m_DetailPivot;



    private PearDetailItem m_DetailItem;

    void Start()
    {
        m_Mask.onClick.AddListener(()=>{
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });
    }

    public void Init()
    {
        InitPears();
    }

    void InitPears()
    {
        var list    = new List<Pear>();
        GameFacade.Instance.DataCenter.Backpack.Pears.ForEach(p => {
            list.Add(p);
        });

        m_PearView.Init(list.Count, (obj, index, is_init)=>{
            PearItem item = obj.transform.GetComponent<PearItem>();
            Pear pear = list[index];
            item.Init(pear);
            

            item.Touch.onClick.RemoveAllListeners();
            item.Touch.onClick.AddListener(()=>{
                ShowDetail(pear);
            });
        });
    }

    void ShowDetail(Pear pear)
    {
        if (m_DetailItem == null) {
            m_DetailItem = GameFacade.Instance.UIManager.LoadItem("PearDetailItem", m_DetailPivot).GetComponent<PearDetailItem>();
        }

        m_DetailItem.Init(pear);
    }
}
