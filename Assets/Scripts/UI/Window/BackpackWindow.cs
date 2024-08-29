using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BackpackWindow : MonoBehaviour
{
    [SerializeField] private Button m_Mask;
    [SerializeField] private SuperScrollView m_PearView;
    [SerializeField] private Transform m_DetailPivot;



    private PearItem m_PearItem;
    private PearDetailItem m_DetailItem;

    private float m_EnterTimer = 0.4f;
    private float m_ExitTimer = 0.2f;
    private bool m_IsAwake = false;

    void Start()
    {
        m_Mask.onClick.AddListener(()=>{
            StartCoroutine(ExitAnim());
        });
    }

    public void Init(Pear current_pear)
    {
        InitPears(current_pear);

        if (!m_IsAwake) StartCoroutine(EnterAnim());

        m_IsAwake = true;
    }

    void InitPears(Pear current_pear)
    {
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
        if (m_DetailItem == null) {
            m_DetailItem = GameFacade.Instance.UIManager.LoadItem("PearDetailItem", m_DetailPivot).GetComponent<PearDetailItem>();

            m_DetailPivot.GetComponent<CanvasGroup>().alpha = 0;
            m_DetailPivot.GetComponent<CanvasGroup>().DOFade(1, m_EnterTimer);
        }

        m_DetailItem.Init(pear);
    }


    #region 协程
    IEnumerator EnterAnim()
    {
        EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, true));


        float origin_y = -800f;
        m_PearView.transform.localPosition = new Vector3(0, origin_y - m_PearView.GetComponent<RectTransform>().rect.height - 100, 0);

        m_PearView.transform.DOLocalMoveY(origin_y, m_EnterTimer).SetEase(Ease.OutCubic);

        yield return new WaitForSeconds(m_EnterTimer);

        EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, false));
        EventManager.SendEvent(new GameEvent(EVENT.UI_BACKPACKOPEN, true));

        yield return null;
    }

    IEnumerator ExitAnim()
    {
        EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, true));


        float origin_y = -800f;

        m_DetailPivot.GetComponent<CanvasGroup>().DOFade(0, m_ExitTimer);

        m_PearView.transform.DOLocalMoveY(origin_y - m_PearView.GetComponent<RectTransform>().rect.height - 100, m_ExitTimer).SetEase(Ease.InCubic);

        yield return new WaitForSeconds(m_ExitTimer);

        EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, false));
        EventManager.SendEvent(new GameEvent(EVENT.UI_BACKPACKOPEN, false));

        GameFacade.Instance.UIManager.UnloadWindow(gameObject);

        yield return null;
    }
    #endregion
}
