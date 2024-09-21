using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PearItem : MonoBehaviour
{
    public Button Touch;

    [SerializeField] private GameObject m_Light;
    [SerializeField] private Image m_Icon;
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

        string color_string = CONST.LEVELCOLOR_PAIRS[m_Pear.Level].Trim('<', '>');

        m_Icon.sprite   = GameFacade.Instance.AssetManager.LoadSprite("Pear" , pear.Class.ToString());
        m_Icon.SetNativeSize();

        m_Icon.GetComponent<ImageOutline>().SetColor(color_string);

        m_Text.text = CONST.LEVELCOLOR_PAIRS[m_Pear.Level] + m_Pear.GetName();

        Select(false);

        FlushUI();
    }

    void FlushUI()
    {
        ShowTagEquip(DataCenter.Instance.User.IsPearEquiped(m_Pear.ID));
    }

    public void ShowTagEquip(bool flag)
    {
        m_TagEquiped.SetActive(flag);
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
