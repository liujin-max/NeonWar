using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    [Header("同时播放数量是否受限制")]
    public int PlayLimit = 99;

    private float m_OriginVolume;
    private AudioSource m_AudioSource;
    public bool AutoDestroy = true;
    private bool m_IsPlayed = false;

    public AudioClip Clip {get { return m_AudioSource.clip;}}

    void Awake()
    {
        m_AudioSource = transform.GetComponent<AudioSource>();
    
        m_OriginVolume= m_AudioSource.volume;
    }

    //因为部分音效绑定在特效上，特效走的是缓存池，所以不能放在Awake里做播放
    void OnEnable()
    {
        if (SoundManager.Instance.IsPlayingSoundFull(m_AudioSource.clip.name)) {
            if (AutoDestroy == true) {             
                Destroy(gameObject); 
            }
            return;
        }

        m_IsPlayed = true;
        m_AudioSource.Play();
        m_AudioSource.volume = m_OriginVolume * GameFacade.Instance.SystemManager.SoundVolume;

        SoundManager.Instance.AddPlayingSound(this);
    }

    void Update()
    {
        if (!m_IsPlayed) return;

        if (m_AudioSource.isPlaying == false) {
            m_IsPlayed = false;
            SoundManager.Instance.ReducePlayingSound(this);

            if (AutoDestroy == true) {             
                Destroy(gameObject); 
            }
        }
    }
}
