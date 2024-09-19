using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BackpackWindow : BaseWindow
{
    [SerializeField] private SuperScrollView m_PearView;
    [SerializeField] private Transform m_PearSeatPivot;
    [SerializeField] private Transform m_DetailPivot;
    [SerializeField] private Transform m_ComposePivot;


    private PearItem m_PearItem;
    private PearDetailItem m_DetailItem;
    private PearComposeItem m_ComposeItem;

    private List<PearItem> m_PearItems = new List<PearItem>();
    private Dictionary<Pear, PearItem> m_ComposeSeats = new Dictionary<Pear, PearItem>();

    private float m_EnterTimer = 0.4f;
    private float m_ExitTimer = 0.2f;
    private bool m_IsAwake = false;




    public void Init(Pear current_pear)
    {
        InitPearSeats();
        InitPears(current_pear);

        if (!m_IsAwake) StartCoroutine(EnterAnim());

        m_IsAwake = true;
    }

    void InitPearSeats()
    {
        var item = GameFacade.Instance.UIManager.LoadItem(DataCenter.Instance.User.CurrentPlayer.UI + "PearItem", m_PearSeatPivot).GetComponent<WeaponPearItem>();
        item.Init();
    }

    void InitPears(Pear current_pear)
    {
        m_PearItems.Clear();

        var list    = new List<Pear>();
        DataCenter.Instance.Backpack.Pears.ForEach(p => {
            list.Add(p);
        });

        //按等级排序
        list.Sort((a1, a2)=>{
            return a2.SortOrder.CompareTo(a1.SortOrder);
        });

        if (m_PearItem != null) {
            m_PearItem.Select(false);
            m_PearItem = null;

        }

        m_PearView.Init(list.Count, (obj, index, is_init)=>{
            PearItem item = obj.transform.GetComponent<PearItem>();
            Pear pear = list[index];
            item.Init(pear);
            item.Select(false);
            m_PearItems.Add(item);
            // Debug.Log("数量：" + m_PearItems.Count);
            

            item.Touch.onClick.RemoveAllListeners();
            item.Touch.onClick.AddListener(()=>{
                if (m_PearItem != null) m_PearItem.Select(false);

                m_PearItem = item;
                m_PearItem.Select(true);

                ShowDetail(pear);
            });

            if (is_init == true && pear == current_pear && m_PearItem == null)
            {
                m_PearItem = item;
                m_PearItem.Select(true);

                ShowDetail(pear);
            }
        });
    }

    public void ShowDetail(Pear pear)
    {
        m_DetailPivot.gameObject.SetActive(true);
        m_ComposePivot.gameObject.SetActive(false);

        if (m_DetailItem == null) {
            m_DetailItem = GameFacade.Instance.UIManager.LoadItem("PearDetailItem", m_DetailPivot).GetComponent<PearDetailItem>();

            m_DetailPivot.GetComponent<CanvasGroup>().alpha = 0;
            m_DetailPivot.GetComponent<CanvasGroup>().DOFade(1, m_EnterTimer);
        }

        m_DetailItem.Init(pear);
    }

    void ShowCompose(Pear pear)
    {
        m_DetailPivot.gameObject.SetActive(false);
        m_ComposePivot.gameObject.SetActive(true);

        if (m_ComposeItem == null) {
            m_ComposeItem = GameFacade.Instance.UIManager.LoadItem("PearComposeItem", m_ComposePivot).GetComponent<PearComposeItem>();

            m_ComposePivot.GetComponent<CanvasGroup>().alpha = 0;
            m_ComposePivot.GetComponent<CanvasGroup>().DOFade(1, m_EnterTimer);
        }

        m_ComposeItem.Init((return_pear)=>{
            m_DetailPivot.gameObject.SetActive(true);
            m_ComposePivot.gameObject.SetActive(false);

            InitPears(return_pear != null ? return_pear : pear);
        });

    }

    void ShowComposeList(Pear pear)
    {
        m_PearItems.Clear();
        m_ComposeSeats.Clear();

        //筛选列表
        var list    = new List<Pear>();
        DataCenter.Instance.Backpack.Pears.ForEach(p => {
            if (p.Level == pear.Level && !DataCenter.Instance.User.IsPearEquiped(p.ID)) {
                for (int i = 0; i < p.Count; i++) {
                    var new_pear = Pear.Create(p.ID, 1);
                    list.Add(new_pear);
                }
            }
        });
        

        m_PearView.Init(list.Count, (obj, index, is_init)=>{
            PearItem item = obj.transform.GetComponent<PearItem>();
            Pear p = list[index];
            item.Init(p);
            item.ShowTagEquip(false);
            item.Select(m_ComposeSeats.ContainsKey(p));
            m_PearItems.Add(item);

            item.Touch.onClick.RemoveAllListeners();
            item.Touch.onClick.AddListener(()=>{
                if (m_ComposeSeats.ContainsKey(p))  //卸下
                {
                    EventManager.SendEvent(new GameEvent(EVENT.UI_COMPOSECHANGE, false, p));
                }
                else    //
                {
                    if (m_ComposeSeats.Count >= CONST.COMPOSE_COUNTMAX) return;

                    EventManager.SendEvent(new GameEvent(EVENT.UI_COMPOSECHANGE, true, p));
                }
            });

            if (m_ComposeSeats.Count == 0 && p.ID == pear.ID)
            {
                EventManager.SendEvent(new GameEvent(EVENT.UI_COMPOSECHANGE, true, p));
            }
        });
    }



    #region 协程
    IEnumerator EnterAnim()
    {
        EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, true));


        m_PearView.transform.localPosition = new Vector3(0, 1500, 0);

        m_PearView.transform.DOLocalMoveY(750, m_EnterTimer).SetEase(Ease.OutCubic);

        yield return new WaitForSeconds(m_EnterTimer);

        EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, false));
        EventManager.SendEvent(new GameEvent(EVENT.UI_BACKPACKOPEN, true));

        yield return null;
    }

    IEnumerator ExitAnim()
    {
        EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, true));

  
        m_DetailPivot.gameObject.SetActive(false);
        m_ComposePivot.gameObject.SetActive(false);
        m_PearSeatPivot.gameObject.SetActive(false);


        m_PearView.transform.DOLocalMoveY(1500f, m_ExitTimer).SetEase(Ease.InCubic);

        yield return new WaitForSeconds(m_ExitTimer);

        EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, false));
        EventManager.SendEvent(new GameEvent(EVENT.UI_BACKPACKOPEN, false));

        GameFacade.Instance.UIManager.UnloadWindow(gameObject);

        yield return null;
    }
    #endregion



    public void OnExit()
    {
        StartCoroutine(ExitAnim());
    }

    public void OnPearComposeStart(Pear pear)
    {
        ShowCompose(pear);
        ShowComposeList(pear);
    }

    public void OnComposeChange(bool flag, Pear pear)
    {
        if (flag == true)
        {
            foreach (var item in m_PearItems)
            {
                if (item.Pear == pear) {
                    item.Select(true);
                    m_ComposeSeats[pear] = item;
                    break;
                }
            }
        }
        else
        {
            if (m_ComposeSeats.ContainsKey(pear))
            {
                m_ComposeSeats[pear].Select(false);
                m_ComposeSeats.Remove(pear);
            }
        }
    }

}
