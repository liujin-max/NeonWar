using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    private AudioSource m_AudioSource;
    private float m_OriginVolume;

    void Awake()
    {
        m_AudioSource = transform.GetComponent<AudioSource>();

        m_AudioSource.Play();

        m_OriginVolume  = m_AudioSource.volume;
        m_AudioSource.volume = m_OriginVolume * GameFacade.Instance.SystemManager.MusicVolume;
    }

    void Update()
    {
        m_AudioSource.volume = m_OriginVolume * GameFacade.Instance.SystemManager.MusicVolume;
    }
}
