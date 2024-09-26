using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffItem : MonoBehaviour
{
    [SerializeField] Image m_Icon;
    [SerializeField] Image m_Mask;
    [SerializeField] TextMeshProUGUI m_Text;
    [SerializeField] TextMeshProUGUI m_Count;

    public Buff Buff;
    private int m_BuffCount;

    public void Init(Buff buff)
    {
        Buff = buff;

        m_Icon.sprite = GameFacade.Instance.AssetManager.LoadSprite("Buff" , buff.ID.ToString());
        m_Icon.SetNativeSize();

        string col  = buff.TYPE == CONST.BUFF_TYPE.DE ? CONST.COLOR_RED : CONST.COLOR_GREEN;
        m_Text.text = col + Buff.Name;

        FlushUI();

        m_BuffCount = buff.ShowValue();
    }

    void FlushUI()
    {
        m_Mask.fillAmount = Buff.Duration.Progress;

        int show_value = Buff.ShowValue();
        if (m_BuffCount != show_value) m_Count.text = show_value > 1 ? show_value.ToString() : "";
    }

    void Update()
    {
        FlushUI();
    }
}
