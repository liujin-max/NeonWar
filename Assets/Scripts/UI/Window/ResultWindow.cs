using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultWindow : BaseWindow
{
    [SerializeField] TextMeshProUGUI m_Glass;
    [SerializeField] GameObject m_WorthPivot;
    [SerializeField] TextMeshProUGUI m_WorthGlass;
    [SerializeField] GameObject m_PearPivot;
    [SerializeField] Transform m_PearContent;
    [SerializeField] GameObject m_Tip;

    [Header("按钮")]
    [SerializeField] Button m_BtnContinue;
    [SerializeField] Button m_BtnVideo;
    


    private Action<int> m_RewardCall;
    private ValueTransition m_GlassValue = new ValueTransition(1f, 0.3f);
    private ValueTransition m_WorthValue = new ValueTransition(1f, 0.3f);



    // Start is called before the first frame update
    void Start()
    {
        m_BtnContinue.onClick.AddListener(()=>{
            Field.Instance.End();
            Field.Instance.Transist(CONST.FSMSTATE.IDLE);
        });

        m_BtnVideo.onClick.AddListener(()=>{
            Platform.Instance.REWARD_VIDEOAD("", ()=>{
                m_RewardCall?.Invoke(2);

                Field.Instance.End();
                Field.Instance.Transist(CONST.FSMSTATE.IDLE);
            });
        });
    }

    public void Init(CONST.RESULT result, int glass, int worth_glass, Action<int> action)
    {
        m_RewardCall = action;

        m_WorthPivot.SetActive(worth_glass > 0);

        m_GlassValue.SetValue(glass);
        m_WorthValue.SetValue(worth_glass);

        m_Tip.SetActive(result == CONST.RESULT.LOSE);
    }

    public void InitPears(List<Pear> pears)
    {
        m_PearPivot.SetActive(pears.Count > 0);

        foreach (var pear in pears)
        {
            var item = GameFacade.Instance.UIManager.LoadItem("PearItem", m_PearContent).GetComponent<PearItem>();
            item.Init(pear);
            item.ShowTagEquip(false);
        }
    }

    void Update()
    {
        m_GlassValue.Update(Time.deltaTime);
        m_WorthValue.Update(Time.deltaTime);
    }

    void LateUpdate()
    {
        m_Glass.text = ToolUtility.FormatNumber(Mathf.FloorToInt(m_GlassValue.Value));
        m_WorthGlass.text = ToolUtility.FormatNumber(Mathf.FloorToInt(m_WorthValue.Value));
    }
}
