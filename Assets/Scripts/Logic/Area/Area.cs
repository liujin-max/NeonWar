using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//场地区域
//区域有各种效果:
//荆棘区域(减速上面的敌人)
//燃烧区域(对上面的敌人持续扣血)
public class Area : MonoBehaviour
{
    //区域的释放者
    [HideInInspector] public Unit Caster;

    //处于区域内的单位
    protected Dictionary<Unit, float> m_Units = new Dictionary<Unit, float>();
    public Dictionary<Unit, float> Units {get { return m_Units;}}

    //持续时间
    protected CDTimer m_Timer = new CDTimer(999999);



    public virtual void Init(Unit caster, float time)
    {
        Caster = caster;

        m_Timer.Reset(time);
    }

    //自定义的Update函数
    public virtual void CustomUpdate(float deltaTime)
    {
        m_Timer.Update(deltaTime);

        foreach (var key in m_Units.Keys.ToList()) m_Units[key] += deltaTime;
    }

    //进入区域
    //进入区域后要做的逻辑写这里面
    public virtual void Enter(Unit unit)
    {

    }

    //离开区域时
    //重写此方法，把离开区域后要做的逻辑写这里面
    public virtual void Exit(Unit unit)
    {

    }

    public bool IsFinished()
    {
        return m_Timer.IsFinished();
    }

    public virtual void Dispose()
    {
        foreach (var unit in m_Units.Keys) Exit(unit);
        m_Units.Clear();

        Destroy(gameObject);
    }

    #region 碰撞检测
    void OnTriggerEnter2D(Collider2D collider)
    {
        var unit = collider.GetComponent<Unit>();
        if (unit == null) return;

        //通常是对敌人生效
        //也不排除后续会对友方生效，例如在地上铺一个治疗区域
        if (unit.Side == Caster.Side) return;
        if (m_Units.ContainsKey(unit)) return;

        Enter(unit);
        m_Units.Add(unit, 0);
    }

    void OnTriggerStay2D(Collider2D collider)
    {

    }

    //离开区域
    void OnTriggerExit2D(Collider2D collider)
    {
        var unit = collider.GetComponent<Unit>();
        if (unit == null) return;

        if (!m_Units.ContainsKey(unit)) return;

        Exit(unit);
        m_Units.Remove(unit);
    }
    #endregion
}
