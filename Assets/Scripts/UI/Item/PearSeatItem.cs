using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PearSeatItem : MonoBehaviour
{
    [SerializeField] private Image m_Frame;
    [SerializeField] private Image m_Icon;
    [SerializeField] private Button m_Touch;


    private Pear m_Pear;


    void Start()
    {
        m_Touch.onClick.AddListener(()=>{
            GameFacade.Instance.UIManager.LoadWindow("BackpackWindow", UIManager.MAJOR).GetComponent<BackpackWindow>().Init();

            // GameFacade.Instance.DataCenter.User.SetPearSeat(0, 20052);

            // EventManager.SendEvent(new GameEvent(EVENT.UI_PEAREQUIP));
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
        }
        else
        {
            // m_Frame.sprite = Resources.Load<Sprite>("");
            // m_Frame.SetNativeSize();

            m_Icon.gameObject.SetActive(false);
        }
        
    }
}
