using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PearItem : MonoBehaviour
{
    public Button Touch;

    [SerializeField] private GameObject m_Light;
    [SerializeField] private Image m_Frame;
    [SerializeField] private Image m_Icon;
    [SerializeField] private TextMeshProUGUI m_Count;
    [SerializeField] private GameObject m_TagEquiped;
    [SerializeField] TextMeshProUGUI m_Text;


    private Pear m_Pear;
    public Pear Pear {get { return m_Pear;}}

    void Awake()
    {
        EventManager.AddHandler(EVENT.UI_PEARCHANGE,    OnPearChange);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_PEARCHANGE,    OnPearChange);
    }

    public void Init(Pear pear)
    {
        m_Pear = pear;

        m_Frame.sprite  = GameFacade.Instance.AssetManager.LoadSprite("Quality" , "Quality_" + pear.Level);
        m_Frame.SetNativeSize();

        m_Icon.sprite   = GameFacade.Instance.AssetManager.LoadSprite("Pear" , pear.Class.ToString());
        m_Icon.SetNativeSize();

        m_Text.text = _C.LEVELCOLOR_PAIRS[m_Pear.Level] + m_Pear.Name;

        Select(false);

        FlushUI();
    }

    void FlushUI()
    {
        m_Count.text = m_Pear.Count > 1 ? m_Pear.Count.ToString() : "";

        ShowTagEquip(DataCenter.Instance.User.IsPearEquiped(m_Pear.Class));
    }

    public void ShowTagEquip(bool flag)
    {
        m_TagEquiped.SetActive(flag);
        m_Count.gameObject.SetActive(!flag);
    }

    public void ShowName(bool flag)
    {
        m_Text.gameObject.SetActive(flag);
    }

    public void Select(bool flag)
    {
        m_Light.SetActive(flag);
    }


    #region 监听事件
    private void OnPearChange(GameEvent @event)
    {
        FlushUI();
    }
    #endregion
}
