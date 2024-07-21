using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//基础属性
public class PlayerATT
{
    public int HP   = 3;
    public int ATK  = 1;
    public CDTimer ASP   = new CDTimer(_C.DEFAULT_ASP);    //攻速 

    public CDTimer InvincibleTimer = new CDTimer(1f);   //无敌时间
}

public class Player : MonoBehaviour
{
    [SerializeField] private Transform m_ShootPivot;
    [SerializeField] private GameObject m_BulletTemplate;

    public PlayerATT ATT = new PlayerATT();


    private float m_Angle;      //角度



    public void Init(float angle)
    {
        m_Angle = angle;

        ATT.InvincibleTimer.Full();

        this.SetPosition(ToolUtility.FindPointOnCircle(Vector2.zero, _C.DEFAULT_RADIUS, angle));
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

    public bool IsDead()
    {
        return ATT.HP <= 0;
    }

    //无敌
    public bool IsInvincible()
    {
        return !ATT.InvincibleTimer.IsFinished();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDead()) return;
        if (Field.Instance.STATE == _C.GAME_STATE.PAUSE) return;

        float deltaTime = Time.deltaTime;

        ATT.InvincibleTimer.Update(deltaTime);

        //攻击间隔
        ATT.ASP.Update(deltaTime);
        if (ATT.ASP.IsFinished() == true) {
            ATT.ASP.Reset();

            Shoot();
        }
    }

    void LateUpdate()
    {
        if (IsDead()) return;
        if (Field.Instance.STATE == _C.GAME_STATE.PAUSE) return;

        //始终朝向圆心
        transform.localEulerAngles = new Vector3(0, 0, m_Angle + 90);
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }

    

    #region 逻辑处理
    //同步最新的加成等级
    public void Sync()
    {
        ATT.ATK = GameFacade.Instance.DataCenter.User.ATK * _C.UPGRADE_ATK;

        //每级提高攻速百分比
        ATT.ASP.Reset(_C.DEFAULT_ASP / (1 + _C.UPGRADE_ASP * (GameFacade.Instance.DataCenter.User.ASP - 1)));
    }

    public void UpdateHP(int value)
    {
        ATT.HP += value;
    }

    void Shoot()
    {
        // var bullet = GameFacade.Instance.UIManager.LoadPrefab("Prefab/Element/Bullet", transform.localPosition, Field.Instance.Land.ENTITY_ROOT).GetComponent<Bullet>();

        var bullet = GameFacade.Instance.PoolManager.AllocateBullet(m_BulletTemplate, Vector3.zero);
        bullet.transform.position = m_ShootPivot.position;
        bullet.Shoot(this, ToolUtility.FindPointOnCircle(Vector2.zero, 1000, m_Angle + 180));
    }
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
