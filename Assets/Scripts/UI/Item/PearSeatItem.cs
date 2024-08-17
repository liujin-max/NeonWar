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
    



    private Pear m_Pear;


    void Start()
    {
        m_Touch.onClick.AddListener(()=>{
            NavigationController.GotoBackpack(m_Pear);
        });
    }

    void OnDestroy()
    {
        AssetsManager.Unload(m_Frame.gameObject);
        AssetsManager.Unload(m_Icon.gameObject);
    }

    public void Init(Pear pear)
    {
        m_Pear = pear;

        FlushUI();
    }

    void FlushUI()
    {
        if (m_Pear != null)
        {
            m_Frame.sprite = AssetsManager.LoadSprite("Quality", "Quality_" + m_Pear.Level, m_Frame.gameObject);
            m_Frame.SetNativeSize();

            m_Icon.gameObject.SetActive(true);
            m_Icon.sprite = AssetsManager.LoadSprite("Pear" , m_Pear.Class.ToString(), m_Icon.gameObject);
            m_Icon.SetNativeSize();

            m_Text.text = _C.LEVELCOLOR_PAIRS[m_Pear.Level] + m_Pear.Name;
        }
        else
        {
            m_Frame.sprite = AssetsManager.LoadSprite("Quality", "Quality_1", m_Frame.gameObject);
            m_Frame.SetNativeSize();

            m_Text.text = "";

            if (GameFacade.Instance.DataCenter.IsPearUnlock() == true)
            {
                m_Icon.gameObject.SetActive(false);
                AssetsManager.Unload(m_Icon.gameObject);
            }
            else
            {
                m_Icon.gameObject.SetActive(true);

                m_Icon.sprite = AssetsManager.LoadSprite("Universal", "Universal_lock", m_Icon.gameObject);
                m_Icon.SetNativeSize();
            }
        }
        
    }
}
