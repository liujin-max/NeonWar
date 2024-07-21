using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_Result;
    [SerializeField] Button m_BtnContinue;


    // Start is called before the first frame update
    void Start()
    {
        m_BtnContinue.onClick.AddListener(()=>{
            Field.Instance.Transist(_C.FSMSTATE.IDLE);
        });
    }

    public void Init(_C.RESULT result)
    {
        m_Result.text = result == _C.RESULT.VICTORY ? "胜利" : "失败";
    }
}
