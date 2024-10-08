using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class SoundManager
{
    private static SoundManager m_Instance;
    public static SoundManager Instance {
        get { 
            if (m_Instance == null) m_Instance = new SoundManager();
            return m_Instance; 
        }
    }

    private GameObject m_BGM = null;

    private int m_PlayingSoundMax = 3;
    private Dictionary<string, int> m_PlayingSounds = new Dictionary<string, int>();



    //加载音效
    public void Load(string path)
    {
        if (this.IsPlayingSoundFull(path)) {
            return;
        }
        

        GameObject.Instantiate(AssetsManager.LoadPrefab("Sound",path), Vector3.zero, Quaternion.identity);
    }

    public bool IsPlayingSoundFull(string path)
    {
        if (m_PlayingSounds.ContainsKey(path))
        {
            return m_PlayingSounds[path] >= m_PlayingSoundMax;
        }
        return false;
    }

    public void AddPlayingSound(string path)
    {
        if (!m_PlayingSounds.ContainsKey(path))
        {
            m_PlayingSounds[path] = 0;
        }

        m_PlayingSounds[path]++;
    }

    public void ReducePlayingSound(string path)
    {
        if (m_PlayingSounds.ContainsKey(path))
        {
            if (m_PlayingSounds[path] > 0) {
                m_PlayingSounds[path]--;
            }
        }
    }

    //播放音乐
    public void PlayBGM(string path)
    {
        if (m_BGM != null) {
            GameObject.Destroy(m_BGM.gameObject);
        }

        m_BGM = GameObject.Instantiate(AssetsManager.LoadPrefab("Music",path), Vector3.zero, Quaternion.identity);

        GameObject.DontDestroyOnLoad(m_BGM);
    }

    public void StopBGM()
    {
        if (m_BGM != null) {
            GameObject.Destroy(m_BGM.gameObject);
        }
        m_BGM = null;
    }
}
