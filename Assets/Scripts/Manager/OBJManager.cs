using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OBJManager: MonoBehaviour
{
    private Transform PoolLayer;

    private Dictionary<int, List<GameObject>> m_ObstaclePool = new Dictionary<int, List<GameObject>>();

    //特效池
    private Dictionary<string, List<GameObject>> m_EffectPool = new Dictionary<string, List<GameObject>>();
    private int m_EffectMax = 3;    //同种特效最多存3个


    void Awake()
    {
        PoolLayer = GameObject.Find("POOL").transform;
    }






    public GameObject AllocateEffect(string effect_path, Vector3 pos)
    {
        GameObject effect = null;

        List<GameObject> effect_list;
        if (m_EffectPool.TryGetValue(effect_path, out effect_list) != true)
        {
            effect_list = new List<GameObject>();
            m_EffectPool.Add(effect_path, effect_list);
        }


        if (effect_list.Count > 0) {
            effect = effect_list[0];
            effect.transform.SetParent(SceneManager.GetActiveScene().GetRootGameObjects()[0].transform);
            effect.transform.localPosition = pos;
            effect_list.RemoveAt(0);
        } else {
            effect = Instantiate(Resources.Load<GameObject>(effect_path), pos, Quaternion.identity);
        }


        Effect effect_cs    = effect.GetComponent<Effect>();
        effect_cs.ResPath   = effect_path;
        effect_cs.Restart();
        effect.SetActive(true);


        return effect;
    }

    public void RecycleEffect(Effect effect)
    {
        List<GameObject> list = m_EffectPool[effect.ResPath];
        if (list.Count >= m_EffectMax) {  //只存3个
            Destroy(effect.gameObject);
            return;
        }
        list.Add(effect.gameObject);

        effect.transform.SetParent(PoolLayer);
        effect.transform.localPosition = Vector3.zero;
        effect.gameObject.SetActive(false);
    }
}
