using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;

//长毛象
//预瞄位置 横冲直撞 撞击造成2伤害
//冲锋时免疫控制、位移
public class Enemy_206 : Enemy
{
    private DetectLineCharge m_Line = null;
    private Vector3 m_StartPos;
    private Vector3 m_EndPos;



    private CHARGE_STATE m_ChargeState = CHARGE_STATE.NONE;
    private float m_ChargeSpeed = 10.0f;
    private CDTimer m_ChargeTimer = new CDTimer(1f);
    private CDTimer m_ChargePrepareTimer = new CDTimer(1.5f);


    public override void DoAttack()
    {
        ChargePrepare();
    }

    void ChargePrepare()
    {
        m_ChargeState = CHARGE_STATE.PREPARE;

        m_StartPos = transform.localPosition;
        m_EndPos = Field.Instance.Player.transform.localPosition;

        Stop();

        ImmuneDisplaceReference++;
        ImmuneControlReference++;

        //预瞄线
        m_Line = GameFacade.Instance.EffectManager.Load(EFFECT.LINECHARGE, m_StartPos, Field.Instance.Land.ELEMENT_ROOT.gameObject).GetComponent<DetectLineCharge>();
        m_Line.Play(m_StartPos, m_EndPos);
    }

    void ChargeStart()
    {
        m_ChargeState = CHARGE_STATE.CHARGING;

        m_ChargeTimer.Reset(Vector2.Distance(m_StartPos, m_EndPos) / m_ChargeSpeed);

        if (m_Line != null)
        {
            m_Line.GetComponent<Effect>().Dispose();
        }

        Roar();
    }

    void ChargeEnd()
    {
        m_ChargeState = CHARGE_STATE.NONE;
        m_ChargePrepareTimer.Reset();

        ImmuneDisplaceReference--;
        ImmuneControlReference--;

        Resume();
        Push(ToolUtility.VectorToAngle(m_EndPos - m_StartPos));
    }

    public override bool CustomUpdate(float deltaTime)
    {
        if (!base.CustomUpdate(deltaTime)) return false;


        if (m_ChargeState == CHARGE_STATE.PREPARE)
        {
            m_ChargePrepareTimer.Update(deltaTime);
            m_Line?.UpdateProgress(m_ChargePrepareTimer.Progress);

            if (m_ChargePrepareTimer.IsFinished() == true)
            {
                ChargeStart();
            }
        }
        else if (m_ChargeState == CHARGE_STATE.CHARGING)
        {
            m_ChargeTimer.Update(deltaTime);

            transform.localPosition = Vector3.Lerp(m_StartPos, m_EndPos, m_ChargeTimer.Progress);

            if (m_ChargeTimer.IsFinished())
            {
                ChargeEnd();
            }
        }


            
    

        return true;
    }
}
