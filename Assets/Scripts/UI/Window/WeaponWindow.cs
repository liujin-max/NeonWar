using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponWindow : MonoBehaviour
{
    [SerializeField] Transform m_WeaponPivot;



    private WeaponItem m_WEAPONITEM = null;


    void Awake()
    {
        EventManager.AddHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
    }

    public void Init()
    {
        InitWeapon();
    }

    void InitWeapon()
    {
        if (m_WEAPONITEM != null) Destroy(m_WEAPONITEM.gameObject);

        m_WEAPONITEM    = GameFacade.Instance.UIManager.LoadItem(DataCenter.Instance.User.CurrentPlayer.UI + "Item", m_WeaponPivot).GetComponent<WeaponItem>();
        m_WEAPONITEM.Init();
    }

    void FlushUI()
    {
        if (m_WEAPONITEM != null) m_WEAPONITEM.FlushUI();
    }



    #region 监听事件
    private void OnUpdateGlass(GameEvent @event)
    {
        FlushUI();
    }

    #endregion
}
