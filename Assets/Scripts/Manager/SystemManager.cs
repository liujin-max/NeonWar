using UnityEngine;
using WeChatWASM;


//系统类
public class SystemManager : MonoBehaviour
{
    //存储键
    public static string KEY_USER       = "KEY_USER";
    public static string KEY_ARCHIVE    = "KEY_ARCHIVE";    //关卡进行中的存档


    public static string KEY_MUSIC      = "KEY_MUSIC";      //音乐
    public static string KEY_SOUND      = "KEY_SOUND";      //音效
    public static string KEY_VIBRATE    = "KEY_VIBRATE";    //振动开关

    private float m_MusicVolume;
    public float MusicVolume {
        get { return m_MusicVolume;}
        set {
            m_MusicVolume = value;
            PlayerPrefs.SetFloat(KEY_MUSIC, m_MusicVolume);
        }
    }

    private float m_SoundVolume;
    public float SoundVolume {
        get { return m_SoundVolume;}
        set {
            m_SoundVolume = value;
            PlayerPrefs.SetFloat(KEY_SOUND, m_SoundVolume);
        }
    }

    private bool m_VibrateFlag = true;
    public bool VibrateFlag {
        get { return m_VibrateFlag;}
        set {
            m_VibrateFlag = value;
            PlayerPrefs.SetFloat(KEY_VIBRATE, m_VibrateFlag ? 1 : 0);
        }
    }

    void Awake()
    {
        m_MusicVolume   = PlayerPrefs.GetFloat(KEY_MUSIC, 1);
        m_SoundVolume   = PlayerPrefs.GetFloat(KEY_SOUND, 1);

        m_VibrateFlag   = PlayerPrefs.GetFloat(KEY_VIBRATE, 1) == 1;
    }


    public int GetIntByKey(string key)
    {
        return PlayerPrefs.GetInt(key, 0);
    }

    public void SetIntByKey(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

}

