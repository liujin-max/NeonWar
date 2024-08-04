using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PearDetailItem : MonoBehaviour
{
    [SerializeField] Image m_Icon;
    [SerializeField] TextMeshProUGUI m_Title;
    [SerializeField] TextMeshProUGUI m_Description;

    [Header("按钮")]
    [SerializeField] Button m_BtnEquip;
    [SerializeField] Button m_BtnUnEquip;
    [SerializeField] Button BtnSynthesis;



    private Pear m_Pear;


    void Awake()
    {
        EventManager.AddHandler(EVENT.UI_PEARCHANGE,    OnPearChange);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_PEARCHANGE,    OnPearChange);
    }

    void Start()
    {
        m_BtnEquip.onClick.AddListener(()=>{
            if (GameFacade.Instance.DataCenter.User.IsPearSeatsFull() == true) {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "宝珠槽已满"));
                return;
            }

            GameFacade.Instance.DataCenter.User.EquipPear(m_Pear.ID);

            EventManager.SendEvent(new GameEvent(EVENT.UI_PEARCHANGE));
        });

        m_BtnUnEquip.onClick.AddListener(()=>{
            GameFacade.Instance.DataCenter.User.UnloadPear(m_Pear.ID);

            EventManager.SendEvent(new GameEvent(EVENT.UI_PEARCHANGE));
        });

        //合成
        BtnSynthesis.onClick.AddListener(()=>{
            
        });
    }

    public void Init(Pear pear)
    {
        m_Pear = pear;

        m_Title.text        = _C.LEVELCOLOR_PAIRS[pear.Level] + pear.Name;
        m_Description.text  = pear.GetDescription();

        m_Icon.sprite = Resources.Load<Sprite>("UI/Pear/" + m_Pear.Class);
        m_Icon.SetNativeSize();

        FlushUI();
    }

    void FlushUI()
    {
        bool is_equip = GameFacade.Instance.DataCenter.User.IsPearEquiped(m_Pear.ID);

        m_BtnEquip.gameObject.SetActive(!is_equip);
        m_BtnUnEquip.gameObject.SetActive(is_equip);

        BtnSynthesis.gameObject.SetActive(!m_Pear.IsLevelMax());
    }


    #region 监听事件
    private void OnPearChange(GameEvent @event)
    {
        FlushUI();
    }
    #endregion
}
