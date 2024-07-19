using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultWindow : MonoBehaviour
{

    [SerializeField] Button m_BtnContinue;


    // Start is called before the first frame update
    void Start()
    {
        m_BtnContinue.onClick.AddListener(()=>{
            Field.Instance.Transist(_C.FSMSTATE.IDLE);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
