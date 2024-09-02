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

        string col  = buff.TYPE == _C.BUFF_TYPE.DE ? _C.COLOR_RED : _C.COLOR_GREEN;
        m_Text.text = col + Buff.Name;

        FlushUI();

        m_BuffCount = buff.Count;
    }

    void FlushUI()
    {
        m_Mask.fillAmount = Buff.Duration.Progress;

        if (m_BuffCount != Buff.Count) m_Count.text = Buff.Count > 1 ? Buff.Count.ToString() : "";
    }

    void Update()
    {
        FlushUI();
    }
}
