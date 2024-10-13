using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PearItem : MonoBehaviour
{
    public Button Touch;

    [SerializeField] GameObject m_Light;
    [SerializeField] Image m_Icon;
    [SerializeField] GameObject m_TagEquiped;
    [SerializeField] TextMeshProUGUI m_Text;
    [SerializeField] GameObject m_NewTag;


    private Pear m_Pear;
    public Pear Pear {get { return m_Pear;}}


    private void OnEnable() {
        Event_PearChange.OnEvent += OnPearChange;
    }

    private void OnDisable() {
        Event_PearChange.OnEvent -= OnPearChange;
    }


    public void Init(Pear pear)
    {
        m_Pear = pear;

        string color_string = CONST.LEVEL_COLOR_PAIRS[m_Pear.Level].Trim('<', '>');

        m_Icon.sprite   = GameFacade.Instance.AssetManager.LoadSprite("Pear" , pear.ID.ToString());
        m_Icon.SetNativeSize();

        m_Icon.GetComponent<ImageOutline>().SetColor(color_string);

        m_Text.text = CONST.LEVEL_COLOR_PAIRS[m_Pear.Level] + m_Pear.Name;

        Select(false);

        FlushUI();
    }

    void FlushUI()
    {
        ShowTagEquip(DataCenter.Instance.User.IsPearEquiped(m_Pear));
        ShowNewTag(m_Pear.IsNew);
    }

    public void ShowTagEquip(bool flag)
    {
        m_TagEquiped.SetActive(flag);
    }

    public void ShowNewTag(bool flag)
    {
        m_NewTag.SetActive(flag);
    }

    public void ShowName(bool flag)
    {
        m_Text.gameObject.SetActive(flag);
    }

    public void Select(bool flag)
    {
        m_Light.SetActive(flag);

        if (flag == true)
        {
            m_Pear.IsNew = false;
            ShowNewTag(false);
        }
    }


    #region 监听事件
    private void OnPearChange(Event_PearChange e)
    {
        FlushUI();
    }
    #endregion
}
