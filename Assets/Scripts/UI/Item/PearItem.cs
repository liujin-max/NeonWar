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


    private Pear m_Pear;

    void Awake()
    {
        EventManager.AddHandler(EVENT.UI_PEARCHANGE,    OnPearChange);
    }

    void OnDestroy()
    {
        AssetsManager.Unload("Quality");
        AssetsManager.Unload("Pear");

        EventManager.DelHandler(EVENT.UI_PEARCHANGE,    OnPearChange);
    }

    public void Init(Pear pear)
    {
        m_Pear = pear;

        m_Frame.sprite  = AssetsManager.LoadSprite("Quality", "Quality_" + pear.Level, m_Frame.gameObject);
        m_Frame.SetNativeSize();

        m_Icon.sprite   = AssetsManager.LoadSprite("Pear", pear.Class.ToString(), m_Icon.gameObject);
        m_Icon.SetNativeSize();

        Select(false);

        FlushUI();
    }

    void FlushUI()
    {
        m_Count.text = m_Pear.Count > 1 ? m_Pear.Count.ToString() : "";

        ShowTagEquip(GameFacade.Instance.DataCenter.User.IsPearEquiped(m_Pear.ID));
    }

    public void ShowTagEquip(bool flag)
    {
        m_TagEquiped.SetActive(flag);
        m_Count.gameObject.SetActive(!flag);
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
