using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBubble : MonoBehaviour
{
    private SpriteRenderer m_Sprite;



    private int m_BuffID;
    private int m_BuffValue;
    private CDTimer m_Timer = new CDTimer(8f);
    private bool m_IsShine = false;

    void Awake ()
    {
        m_Sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    public void Init(int id, int value)
    {
        m_BuffID    = id;
        m_BuffValue = value;

        m_Sprite.sprite = GameFacade.Instance.AssetManager.LoadSprite("Buff" , id.ToString());
    }

    void Shine()
    {
        if (m_IsShine) return;

        m_IsShine = true;

        m_Sprite.GetComponent<Animation>().Play("SpriteShine");
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }

    public bool IsFinished()
    {
        return m_Timer.IsFinished();
    }

    public void CustomUpdate(float deltaTime)
    {
        //生命周期
        m_Timer.Update(Time.deltaTime);

        if (m_Timer.Progress >= 0.7f) Shine();
    }

    #region 碰撞检测
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag != "Player") return;

        SoundManager.Instance.Load(SOUND.BUFF);

        //缺少特效
        collider.GetComponent<Unit>().AddBuff(m_BuffID, m_BuffValue);
        Field.Instance.RemoveBuffBubble(this);
    }

    #endregion

}
