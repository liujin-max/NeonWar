using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PearDetailItem : MonoBehaviour
{
    [SerializeField] Image m_Icon;
    [SerializeField] TextMeshProUGUI m_Title;
    [SerializeField] Transform m_PropertyPivot;
    [SerializeField] TextMeshProUGUI m_Description;

    [Header("按钮")]
    [SerializeField] Button m_BtnEquip;
    [SerializeField] Button m_BtnUnEquip;
    [SerializeField] Button BtnSynthesis;



    private Pear m_Pear;
    private List<PropertyItem> m_PropertyItems = new List<PropertyItem>();
    private bool IsComposeState = false;

    PropertyItem new_property_item(int order)
    {
        PropertyItem item = null;
        if (m_PropertyItems.Count > order) {
            item = m_PropertyItems[order];
        }
        else {
            item = GameFacade.Instance.UIManager.LoadItem("PropertyItem", m_PropertyPivot).GetComponent<PropertyItem>();
            m_PropertyItems.Add(item);
        }
        item.gameObject.SetActive(true);

        return item;
    }

    void Awake()
    {
        EventManager.AddHandler(EVENT.UI_PEARCHANGE,    OnPearChange);

        m_BtnEquip.onClick.AddListener(()=>{
            SoundManager.Instance.Load(SOUND.CLICK);

            if (!DataCenter.Instance.User.HasSamePear(m_Pear) && DataCenter.Instance.User.IsPearSeatsFull() == true) {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "装备槽已满"));
                return;
            }

            var slot = DataCenter.Instance.User.EquipPear(m_Pear);

            EventManager.SendEvent(new GameEvent(EVENT.UI_PEARCHANGE, true, m_Pear, slot));
        });

        m_BtnUnEquip.onClick.AddListener(()=>{
            SoundManager.Instance.Load(SOUND.CLICK);

            var slot = DataCenter.Instance.User.UnloadPear(m_Pear);

            EventManager.SendEvent(new GameEvent(EVENT.UI_PEARCHANGE, false, m_Pear, slot));
        });

        //合成
        BtnSynthesis.onClick.AddListener(()=>{
            SoundManager.Instance.Load(SOUND.CLICK);

            EventManager.SendEvent(new GameEvent(EVENT.UI_PEARCOMPOSE, m_Pear));
        });
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_PEARCHANGE,    OnPearChange);
    }


    public void Init(Pear pear, bool is_compose_state = false)
    {
        m_Pear = pear;
        IsComposeState = is_compose_state;

        string color_string = CONST.LEVEL_COLOR_PAIRS[pear.Level];

        m_Title.text = color_string + pear.Name;

        m_Icon.sprite = GameFacade.Instance.AssetManager.LoadSprite("Pear" , m_Pear.ID.ToString());
        m_Icon.SetNativeSize();

        m_Icon.GetComponent<ImageOutline>().SetColor(color_string.Trim('<', '>'));


        InitProperties(pear);
        InitSpecialProperty(pear);
        FlushUI();

        SoundManager.Instance.Load(SOUND.PEARCLICK);
    }

    void InitProperties(Pear pear)
    {
        foreach (var item in m_PropertyItems) item.gameObject.SetActive(false);

        for (int i = 0; i < pear.Properties.Count; i++)
        {
            var property = pear.Properties[i];
            var item = new_property_item(i);
            item.Init(property);
        }
        
    }

    void InitSpecialProperty(Pear pear)
    {
        if (pear.SpecialProperty == null)
        {
            m_Description.text = "";
            return;
        }

        string text_color = pear.SpecialProperty.IsValid ? "<#F18423>" : "<#9C9C9C>";

        m_Description.text = string.Format("{0}{1}:{2}", text_color , pear.SpecialProperty.Name, pear.SpecialProperty.GetDescription());
    }

    void FlushUI()
    {
        bool is_equip = DataCenter.Instance.User.IsPearEquiped(m_Pear);

        m_BtnEquip.gameObject.SetActive(!IsComposeState && !is_equip);
        m_BtnUnEquip.gameObject.SetActive(!IsComposeState && is_equip);

        //装备着的宝珠无法合成
        BtnSynthesis.gameObject.SetActive(!IsComposeState && !m_Pear.IsLevelMax() && !is_equip);
    }


    #region 监听事件
    private void OnPearChange(GameEvent @event)
    {
        FlushUI();

        InitSpecialProperty(m_Pear);
    }
    #endregion
}
