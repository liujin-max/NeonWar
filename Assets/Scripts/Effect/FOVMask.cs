using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVMask : MonoBehaviour
{
    [SerializeField]
    Transform m_FOV;


    private Transform m_Follower;


    public void Init(Transform follower)
    {
        m_Follower = follower;

        m_FOV.GetComponent<Animation>().Play("FOV_Show");

        Follow();
    }

    void Follow()
    {
        if (m_Follower == null) return;

        m_FOV.position = m_Follower.position;
        m_FOV.rotation = m_Follower.rotation;
    }

    private void LateUpdate() {
        Follow();
    }

    public void Dispose()
    {
        StartCoroutine(Exit());
    }

    IEnumerator Exit()
    {
        m_FOV.GetComponent<Animation>().Play("FOV_Hide");

        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }
}
