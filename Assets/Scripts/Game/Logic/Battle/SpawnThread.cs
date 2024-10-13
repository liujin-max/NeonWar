using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


//负责单个怪物的出场
public class SpawnThread
{
    private bool m_FinishFlag;
    public bool IsFinished {get{ return m_FinishFlag; }}

    private MonsterJSON m_JSON;
    private Enemy m_Enemy = null;

    private CDTimer m_Timer = new CDTimer(0.6f);

    public void Start(List<Enemy> enemies, MonsterJSON monsterJSON, Vector2 point)
    {
        m_JSON = monsterJSON;

        //黑洞动画
        var hole = GameFacade.Instance.EffectManager.Load(EFFECT.BLACKHOLE, point, Field.Instance.Land.ELEMENT_ROOT.gameObject).GetComponent<BlackHole>();
        hole.Show();

        GameFacade.Instance.AssetManager.AsyncLoadPrefab("Prefab/Enemy/" + monsterJSON.ID, point, Field.Instance.Land.ENEMY_ROOT, (obj)=>{
            m_Enemy = obj.GetComponent<Enemy>();
            m_Enemy.gameObject.SetActive(false);
            m_Enemy.SetValid(false);
            m_Enemy.InitBlackHole(hole);
            enemies.Add(m_Enemy);

            if (monsterJSON.Type == ENEMY_TYPE.BOSS) m_Enemy.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
        });
    }

    public void CustomUpdate(float dt)
    {
        if (m_Enemy == null) return;
        
        m_Timer.Update(dt);
        if (!m_Timer.IsFinished()) return;

        m_Enemy.gameObject.SetActive(true);
        m_Enemy.SetValid(true);
        m_Enemy.Init(m_JSON);
        m_Enemy.Appear();
        m_Enemy.Push(RandomUtility.Random(0, 360));

        m_FinishFlag = true;
    }

    public void Dispose()
    {
        if (m_Enemy != null) GameObject.Destroy(m_Enemy.gameObject);
    }
}
