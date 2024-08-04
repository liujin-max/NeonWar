using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PearItem : MonoBehaviour
{
    [SerializeField] private Image m_Icon;
    [SerializeField] private TextMeshProUGUI m_Count;
    [SerializeField] private GameObject m_TagEquiped;


    private Pear m_Pear;

    public void Init(Pear pear)
    {
        m_Pear = pear;

        m_Icon.sprite = Resources.Load<Sprite>("UI/Pear/" + m_Pear.Class);
        m_Icon.SetNativeSize();

        FlushUI();
    }

    void FlushUI()
    {
        m_Count.text = m_Pear.Count > 1 ? m_Pear.Count.ToString() : "";
    }

    public void ShowTagEquip(bool flag)
    {
        m_TagEquiped.SetActive(flag);
        m_Count.gameObject.SetActive(!flag);
    }
}
