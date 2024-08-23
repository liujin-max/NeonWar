using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PearSeatItem : MonoBehaviour
{
    [SerializeField] Button m_Touch;
    [SerializeField] Image m_Frame;
    [SerializeField] Image m_Icon;
    [SerializeField] TextMeshProUGUI m_Text;
    [SerializeField] GameObject m_LockTag;
    



    private Pear m_Pear;


    void Start()
    {
        m_Touch.onClick.AddListener(()=>{
            NavigationController.GotoBackpack(m_Pear);
        });
    }

    public void Init(Pear pear)
    {
        m_Pear = pear;

        FlushUI();
    }

    void FlushUI()
    {
        m_LockTag.SetActive(false);

        if (m_Pear != null)
        {
            AssetManager.LoadSprite(m_Frame, "Quality", "Quality_" + m_Pear.Level);

            m_Icon.gameObject.SetActive(true);
            AssetManager.LoadSprite(m_Icon, "Pear", m_Pear.Class.ToString());

            m_Text.text = _C.LEVELCOLOR_PAIRS[m_Pear.Level] + m_Pear.Name;
        }
        else
        {
            m_Icon.gameObject.SetActive(false);

            AssetManager.LoadSprite(m_Frame, "Quality", "Quality_1");

            m_Text.text = "";

            m_LockTag.SetActive(!DataCenter.Instance.IsPearUnlock());
        }
        
    }
}
