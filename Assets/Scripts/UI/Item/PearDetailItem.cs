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
            if (DataCenter.Instance.User.IsPearSeatsFull() == true) {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "宝珠槽已满"));
                return;
            }

            DataCenter.Instance.User.EquipPear(m_Pear);

            EventManager.SendEvent(new GameEvent(EVENT.UI_PEARCHANGE));
        });

        m_BtnUnEquip.onClick.AddListener(()=>{
            DataCenter.Instance.User.UnloadPear(m_Pear.ID);

            EventManager.SendEvent(new GameEvent(EVENT.UI_PEARCHANGE));
        });

        //合成
        BtnSynthesis.onClick.AddListener(()=>{
            EventManager.SendEvent(new GameEvent(EVENT.UI_PEARCOMPOSE, m_Pear));
        });
    }

    public void Init(Pear pear)
    {
        m_Pear = pear;

        string color_string = CONST.LEVELCOLOR_PAIRS[pear.Level];

        m_Title.text = color_string + pear.GetName();
        m_Description.text  = pear.GetDescription();

        m_Icon.sprite = GameFacade.Instance.AssetManager.LoadSprite("Pear" , m_Pear.Class.ToString());
        m_Icon.SetNativeSize();

        m_Icon.GetComponent<ImageOutline>().SetColor(color_string.Trim('<', '>'));

        FlushUI();
    }

    void FlushUI()
    {
        bool is_equip = DataCenter.Instance.User.IsPearEquiped(m_Pear.ID);

        m_BtnEquip.gameObject.SetActive(!is_equip);
        m_BtnUnEquip.gameObject.SetActive(is_equip);

        //装备着的宝珠无法合成
        BtnSynthesis.gameObject.SetActive(!m_Pear.IsLevelMax() && !is_equip);
    }


    #region 监听事件
    private void OnPearChange(GameEvent @event)
    {
        FlushUI();
    }
    #endregion
}
