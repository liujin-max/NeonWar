using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_Glass;
    [SerializeField] GameObject m_WorthPivot;
    [SerializeField] TextMeshProUGUI m_WorthGlass;
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
            m_RewardCall?.Invoke(1);

            Field.Instance.Transist(_C.FSMSTATE.IDLE);
        });

        m_BtnVideo.onClick.AddListener(()=>{
            Platform.Instance.REWARD_VIDEOAD("", ()=>{
                m_RewardCall?.Invoke(3);

                Field.Instance.Transist(_C.FSMSTATE.IDLE);
            });
        });
    }

    public void Init(_C.RESULT result, int glass, int worth_glass, Action<int> action)
    {
        m_RewardCall = action;

        m_WorthPivot.SetActive(worth_glass > 0);

        m_GlassValue.SetValue(glass);
        m_WorthValue.SetValue(worth_glass);

        m_Tip.SetActive(result == _C.RESULT.LOSE);
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
