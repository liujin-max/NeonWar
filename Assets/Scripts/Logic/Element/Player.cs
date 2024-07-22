using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : Unit
{
    [SerializeField] private Transform m_PartsPivot;


    public CDTimer InvincibleTimer = new CDTimer(1f);   //无敌时间

    private List<Parts> m_Parts = new List<Parts>();



    public void Init(float angle)
    {
        Side    = _C.SIDE.PLAYER;
        m_Angle = angle;

        InvincibleTimer.Full();

        this.SetPosition(ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, angle));

        InitParts();
    }

    //加载配件
    void InitParts()
    {
        var p1 = GameFacade.Instance.UIManager.LoadPrefab("Prefab/Parts/Parts_1000", new Vector2(-0.7f, 0.6f), m_PartsPivot.transform).GetComponent<Parts>();
        p1.Init(this);
        p1.Sync();
        m_Parts.Add(p1);


        var p2 = GameFacade.Instance.UIManager.LoadPrefab("Prefab/Parts/Parts_1000", new Vector2( 0.7f, 0.6f), m_PartsPivot.transform).GetComponent<Parts>();
        p2.Init(this);
        p2.Sync();
        m_Parts.Add(p2);
    }

    //angle : 0 -> 360
    public void Move(float direction)
    {
        float speed = 2;

        float t = Mathf.Clamp01(Time.deltaTime / 0.01f);
        m_Angle = Mathf.LerpAngle(m_Angle, m_Angle + direction * speed, t);

        this.SetPosition(ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, m_Angle));
    }

    //对角线闪现
    public void Blink()
    {
        m_Angle += 180;

        this.SetPosition(ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, m_Angle));
    }

    public void SetPosition(Vector2 pos)
    {
        transform.localPosition = pos;
    }

    //无敌
    public bool IsInvincible()
    {
        return !InvincibleTimer.IsFinished();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (IsDead()) return;
        if (Field.Instance.STATE == _C.GAME_STATE.PAUSE) return;

        float deltaTime = Time.deltaTime;

        InvincibleTimer.Update(deltaTime);

        base.Update();
    }


    void LateUpdate()
    {
        //始终朝向圆心
        transform.localEulerAngles = new Vector3(0, 0, m_Angle + 90);
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }

    

    #region 逻辑处理


    #endregion


    #region 碰撞检测
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            Field.Instance.Crash(collider.gameObject.GetComponent<Enemy>(), this);
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {

    }

    void OnTriggerExit2D(Collider2D collider)
    {

    }
    #endregion
}
