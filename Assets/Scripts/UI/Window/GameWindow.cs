using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{
    [SerializeField] GameObject m_Joystick;
    [SerializeField] LongPressButton m_BtnLeft;
    [SerializeField] LongPressButton m_BtnRight;


    [SerializeField] TextMeshProUGUI m_Glass;


    private float LeftPressTime = 0;
    private float RightPressTime= 0;

    void Awake()
    {
        EventManager.AddHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONUPDATEGLASS,    OnUpdateGlass);
    }


    //同时按下的时间间隔小于0.05秒
    bool CheckBlink()
    {
        return LeftPressTime > 0 && RightPressTime > 0 && Mathf.Abs(LeftPressTime - RightPressTime) <= 0.05f;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_BtnLeft.SetCallback(
        ()=>{
            Field.Instance.LeftBtnPressFlag = true;
            LeftPressTime   = Time.time;

            if (CheckBlink()) {
                LeftPressTime = 0;
                RightPressTime= 0;
                EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_DOUBLE));
            }
            else EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        }, 
        ()=>{Field.Instance.LeftBtnPressFlag = false;}, 
        ()=>{
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        });

        m_BtnRight.SetCallback(
        ()=>{
            Field.Instance.RightBtnPressFlag = true;
            RightPressTime = Time.time;

            if (CheckBlink()) {
                LeftPressTime = 0;
                RightPressTime= 0;
                EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_DOUBLE));
            }
            else EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));
        }, 
        ()=>{Field.Instance.RightBtnPressFlag = false;}, 
        ()=>{
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));
        });
    }

    void Update()
    {
        //监听电脑端的输入
        // 检测方向键输入
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Field.Instance.LeftBtnPressFlag = true;
            LeftPressTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow)) { Field.Instance.LeftBtnPressFlag = false;}

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Field.Instance.RightBtnPressFlag = true;
            RightPressTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.RightArrow)) { Field.Instance.RightBtnPressFlag = false;}

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));
        }

        if (CheckBlink() == true)
        {
            LeftPressTime = 0;
            RightPressTime = 0;
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_DOUBLE));
        }
    }


    public void ShowJoyStick(bool flag)
    {
        m_Joystick.gameObject.SetActive(flag);
    }




    #region 监听事件
    private void OnUpdateGlass(GameEvent @event)
    {
        int offset  = (int)@event.GetParam(0);

        m_Glass.text = (GameFacade.Instance.DataCenter.User.Glass + offset).ToString();
    }
    #endregion
}
