using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraUtility : MonoBehaviour
{
    private Camera m_Camera;

    private Tweener m_SmallShakeTweener;
    private Tweener m_ShakeTweener;

    // Start is called before the first frame update
    void Awake()
    {
        m_Camera = this.GetComponent<Camera>();

        FilterOrthographic(m_Camera);
    }

    public void Reset()
    {
        FilterOrthographic(m_Camera);
    }

    void FilterOrthographic(Camera camera)
    {
        ScreenOrientation designAutoRotation = Screen.orientation;

        float aspect = camera.aspect;
        float designOrthographicSize = 9.6f;
        float designHeight = 1920f;
        float designWidth = 1080f;
        float designAspect = designWidth / designHeight;
        float widthOrthographicSize = designOrthographicSize * designAspect;
        switch (designAutoRotation)
        {
            case ScreenOrientation.Portrait:
                if(aspect < designAspect)
                {
                    camera.orthographicSize = widthOrthographicSize / aspect;
                } 
                else
                {
                    camera.orthographicSize = designOrthographicSize;
                    //_camera.orthographicSize = designOrthographicSize * (aspect / designAspect);
                }
                break;
            case ScreenOrientation.AutoRotation:
                break;
            case ScreenOrientation.LandscapeLeft:
                break;
            case ScreenOrientation.LandscapeRight:
                break;
            case ScreenOrientation.PortraitUpsideDown:
                break;
            default:
                break;

        }
    }

    void FixedUpdate()
    {
        FilterOrthographic(m_Camera);
    }

    public void DoShake()
    {
        Platform.Instance.VIBRATE(CONST.VIBRATELEVEL.HEAVY);

        if (m_ShakeTweener != null) {
            transform.localPosition = new Vector3(0, 0, -10);
            m_ShakeTweener.Restart();
            return;
        }

        m_ShakeTweener = transform.DOShakePosition(0.4f, 0.3f, 15, 60).SetAutoKill(false);
    }

    public void DoSmallShake()
    {
        Platform.Instance.VIBRATE(CONST.VIBRATELEVEL.HEAVY);

        if (m_SmallShakeTweener != null) {
            transform.localPosition = new Vector3(0, 0, -10);
            m_SmallShakeTweener.Restart();
            return;
        }

        m_SmallShakeTweener = transform.DOShakePosition(0.1f, 0.1f, 35, 60).SetAutoKill(false);
    }
}
