using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMaskUtility : MonoBehaviour
{
    private Material m_Material;


    void Awake()
    {
        m_Material = transform.GetComponent<Image>().material;
    }

    public void SetCenter(Vector3 pos)
    {
        m_Material.SetVector("_Center", pos);
    }
}
