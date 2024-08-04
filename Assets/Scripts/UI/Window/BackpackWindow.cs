using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackWindow : MonoBehaviour
{
    [SerializeField] private Button m_Mask;
    [SerializeField] private SuperScrollView m_PearView;


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
            item.ShowTagEquip(GameFacade.Instance.DataCenter.User.IsPearEquiped(pear.ID));
        });
    }
}
