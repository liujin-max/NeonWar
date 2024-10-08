using System.Collections;
using System.Collections.Generic;
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
    protected List<Unit> m_Units = new List<Unit>();
    //持续时间
    protected CDTimer m_Timer = new CDTimer(999999);



    public virtual void Init(Unit caster, float time)
    {
        Caster = caster;

        m_Timer.Reset(time);
    }

    //自定义的Update函数
    public void CustomUpdate(float deltaTime)
    {
        m_Timer.Update(deltaTime);
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
        m_Units.ForEach(unit => Exit(unit));
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
        if (m_Units.Contains(unit)) return;

        Enter(unit);
        m_Units.Add(unit);
    }

    void OnTriggerStay2D(Collider2D collider)
    {

    }

    //离开区域
    void OnTriggerExit2D(Collider2D collider)
    {
        var unit = collider.GetComponent<Unit>();
        if (unit == null) return;

        if (m_Units.Contains(unit))
        {
            Exit(unit);
            m_Units.Remove(unit);
        }
    }
    #endregion
}
