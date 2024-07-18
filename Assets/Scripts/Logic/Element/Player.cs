using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //角度
    private float m_Angle;
    //目标角度
    private float m_TAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    

    public void Init(float angle)
    {
        m_Angle = angle;
        m_TAngle= angle;

        this.SetPosition(ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, angle));
    }


    //angle : 0 -> 360
    public void Move(float angle)
    {
        m_TAngle = angle;
    }

    public void SetPosition(Vector2 pos)
    {
        transform.localPosition = pos;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Angle != m_TAngle)
        {
            float t = Mathf.Clamp01(Time.deltaTime / 0.06f);
            m_Angle = Mathf.LerpAngle(m_Angle, m_TAngle, t);

            // 如果已经达到目标角度，重置时间
            if (Mathf.Abs(Mathf.DeltaAngle(m_Angle, m_TAngle)) <= 0.1f) {
                m_Angle = m_TAngle;
            }

            this.SetPosition(ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, m_Angle));
        }
    }

    void LateUpdate()
    {
        //始终朝向圆心
        transform.localEulerAngles = new Vector3(0, 0, m_Angle + 90);
    }


    #region 碰撞检测
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 当碰撞开始时调用
        Debug.Log("Collision Enter with " + collision.gameObject.name);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // 当碰撞持续时调用
        Debug.Log("Collision Stay with " + collision.gameObject.name);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // 当碰撞结束时调用
        Debug.Log("Collision Exit with " + collision.gameObject.name);
    }
    #endregion
}
