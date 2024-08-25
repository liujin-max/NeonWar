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

    public Buff Buff;

    public void Init(Buff buff)
    {
        Buff = buff;

        m_Icon.sprite = GameFacade.Instance.AssetManager.LoadSprite("Buff" , buff.ID.ToString());
        m_Icon.SetNativeSize();

        string col  = buff.TYPE == _C.BUFF_TYPE.DE ? _C.COLOR_RED : _C.COLOR_GREEN;
        m_Text.text = col + Buff.Name;

        FlushUI();
    }

    void FlushUI()
    {
        m_Mask.fillAmount = Buff.Duration.Progress;
    }

    void Update()
    {
        FlushUI();
    }
}
