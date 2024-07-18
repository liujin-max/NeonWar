using System;
using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class ShakeText : MonoBehaviour
{
    TextMeshProUGUI m_text;
    /// <summary>
    /// 速度（时间间隔）
    /// </summary>
    public float shakeSpeed = 0.4f;

    /// <summary>
    /// 幅度
    /// </summary>
    public float shakeAmount = 1f;
    public bool IsAuto = false;

    private Vector3[] m_rawVertex;

    private void Awake()
    {
        m_text = this.GetComponent<TextMeshProUGUI>();

        if (IsAuto == true) {
            Shake();
        }
    }

    public void Clear()
    {
        m_text.text = "";
    }

    public void SetText(string text)
    {
        if (m_text.text.Equals(text)) return ;

        m_text.text = text;
        Shake();
    }

    public void Shake()
    {
        m_text.ForceMeshUpdate();

        GetRawVertex();

        StopAllCoroutines();
        StartCoroutine(DoShake());
    }

    private void GetRawVertex()
    {
        if (m_rawVertex != null) {
            Array.Clear(m_rawVertex, 0, m_rawVertex.Length);
        }
        

        if(m_text.textInfo.characterCount > 0)
        {
            TMP_CharacterInfo charInfo = m_text.textInfo.characterInfo[0];
            TMP_MeshInfo meshInfo = m_text.textInfo.meshInfo[charInfo.materialReferenceIndex];
            //创建对象来保存初始值
            m_rawVertex = new Vector3[meshInfo.vertices.Length];
            for (int i = 0; i < meshInfo.vertices.Length; i++)
            {
                m_rawVertex[i] = new Vector3(meshInfo.vertices[i].x, meshInfo.vertices[i].y, meshInfo.vertices[i].z);
            }
        }
        else
        {
            Debug.LogError("GetRawVertex Failed.");
        }
    }

    IEnumerator DoShake()
    {
        while (true)
        {
            int last_index = 0;
            for (int i = 0; i < m_text.textInfo.characterCount; i++)
            {
                // 获取字符信息和MeshInfo
                TMP_CharacterInfo currentCharInfo = m_text.textInfo.characterInfo[i];
                TMP_MeshInfo meshInfo = m_text.textInfo.meshInfo[currentCharInfo.materialReferenceIndex];
                int vertexCount;
                if (i < m_text.textInfo.characterCount - 1)
                {
                    TMP_CharacterInfo nextCharInfo = m_text.textInfo.characterInfo[i + 1];
                    vertexCount = nextCharInfo.vertexIndex - currentCharInfo.vertexIndex;
                }
                else
                {
                    vertexCount = meshInfo.vertices.Length - currentCharInfo.vertexIndex;
                }

                // 获取起始顶点索引
                int vertexIndex = currentCharInfo.vertexIndex;

                // 随机生成位移量
                int mult = 100;
                float xOffset = GetRandom((int)-shakeAmount * mult, (int)shakeAmount * mult, i);
                float yOffset = GetRandom((int)-shakeAmount * mult, (int)shakeAmount * mult, i + m_text.textInfo.characterCount);
                Vector3 offset = new Vector3(xOffset, yOffset) / 100f;
                //print(xOffset + ", " + yOffset);

                // 顶点偏移
                Vector3[] vertices = meshInfo.vertices;
                // if (vertexIndex == 0 && vertexCount == 0) {
                if (vertices.Length != m_rawVertex.Length) {
                    vertexIndex = last_index + 4;
                    vertexCount = 4;
                } 

                vertexCount = Math.Abs(vertexCount);

                // Debug.Log("长度:" + vertices.Length + ", " + m_rawVertex.Length);
                // Debug.Log("序列：" + vertexIndex + ", " + (vertexIndex + vertexCount));
                // Debug.Log("vertexCount ：" + vertexCount);
                
                for (int j = vertexIndex; j < vertexIndex + vertexCount; j++)
                {
                    if (j < vertices.Length)
                    {
                        vertices[j] = m_rawVertex[j] + offset;
                    }
                }

                if (vertexCount > 0) {
                    last_index = vertexIndex;
                }
                
            }

            m_text.UpdateVertexData();
            yield return new WaitForSeconds(shakeSpeed);
        }
    }

    /// <summary>
    /// 产生随机数
    /// 调用它就可以产生你要的随机数了,如果有需求可以自己重载
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="Max">最大值</param>
    /// <returns></returns>
    public float GetRandom(int min, int Max, int iSeed)
    {
        System.Random rd = new System.Random();
        return (rd.Next(min, Max));
    }
}
