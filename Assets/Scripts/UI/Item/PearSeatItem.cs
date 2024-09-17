using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PearSeatItem : MonoBehaviour
{
    [SerializeField] Button m_Touch;
    [SerializeField] Image m_Icon;
    [SerializeField] TextMeshProUGUI m_Text;
    [SerializeField] GameObject m_LockTag;
    

    private Pear m_Pear;
    public Pear Pear {get { return m_Pear;}}


    void Awake()
    {
        m_Touch.onClick.AddListener(OnClick);
    }

    public void Init(Pear pear)
    {
        m_Pear = pear;

        FlushUI();
    }

    public void RegisterTouchListener(UnityAction callback)
    {
        m_Touch.onClick.RemoveAllListeners();
        m_Touch.onClick.AddListener(callback);
    }

    void FlushUI()
    {
        m_LockTag.SetActive(false);

        if (m_Pear != null)
        {
            string color_string = CONST.LEVELCOLOR_PAIRS[m_Pear.Level].Trim('<', '>');

            m_Icon.gameObject.SetActive(true);
            m_Icon.sprite = GameFacade.Instance.AssetManager.LoadSprite("Pear" , m_Pear.Class.ToString());
            m_Icon.SetNativeSize();

            m_Icon.GetComponent<ImageOutline>().SetColor(color_string);

            m_Text.text = CONST.LEVELCOLOR_PAIRS[m_Pear.Level] + m_Pear.GetName();
        }
        else
        {
            m_Icon.gameObject.SetActive(false);

            m_Text.text = "";

            m_LockTag.SetActive(!DataCenter.Instance.IsPearUnlock());
        }
        
    }


    void OnClick()
    {
        if (m_Pear == null) return;

        EventManager.SendEvent(new GameEvent(EVENT.UI_SELECTPEAR, m_Pear));
    }
}
