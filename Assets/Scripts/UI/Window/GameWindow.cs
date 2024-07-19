using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{
    [SerializeField] GameObject m_Joystick;
    [SerializeField] LongPressButton m_BtnLeft;
    [SerializeField] LongPressButton m_BtnRight;


    private bool IsLeftPress    = false;
    private bool IsRightPress   = false;

    private float LeftPressTime = 0;
    private float RightPressTime= 0;

    void Awake()
    {

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
            LeftPressTime = Time.time;

            if (CheckBlink()) {
                LeftPressTime = 0;
                RightPressTime= 0;
                EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_DOUBLE));
            }
            else EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        }, 
        ()=>{}, 
        ()=>{
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS, -1f));
        });

        m_BtnRight.SetCallback(
        ()=>{
            RightPressTime = Time.time;

            if (CheckBlink()) {
                LeftPressTime = 0;
                RightPressTime= 0;
                EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_DOUBLE));
            }
            else EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));
        }, 
        ()=>{}, 
        ()=>{
            EventManager.SendEvent(new GameEvent(EVENT.ONJOYSTICK_PRESS,  1f));
        });
    }

    void Update()
    {
        //监听电脑端的输入
        // 检测方向键输入
        if (Input.GetKeyDown(KeyCode.LeftArrow))    LeftPressTime = Time.time;
        if (Input.GetKeyDown(KeyCode.RightArrow))   RightPressTime = Time.time;

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
}
