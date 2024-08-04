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

    public void Init(Pear pear)
    {
        m_Pear = pear;

        FlushUI();
    }

    void FlushUI()
    {
        if (m_Pear != null)
        {
            // m_Frame.sprite = Resources.Load<Sprite>("");
            // m_Frame.SetNativeSize();

            m_Icon.gameObject.SetActive(true);
            m_Icon.sprite = Resources.Load<Sprite>("UI/Pear/" + m_Pear.Class);
            m_Icon.SetNativeSize();

            m_Text.text = _C.LEVELCOLOR_PAIRS[m_Pear.Level] + m_Pear.Name;
        }
        else
        {
            m_Frame.sprite = Resources.Load<Sprite>("UI/Game/Game_pear_frame");
            m_Frame.SetNativeSize();

            m_Text.text = "";

            if (GameFacade.Instance.DataCenter.IsPearUnlock() == true)
            {
                m_Icon.gameObject.SetActive(false);
            }
            else
            {
                m_Icon.gameObject.SetActive(true);

                m_Icon.sprite = Resources.Load<Sprite>("UI/Universal/Universal_lock");
                m_Icon.SetNativeSize();
            }
        }
        
    }
}
