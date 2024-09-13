using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;



public class SocketTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("结果：" + Rotate(new int[][] {}));
        // Rotate(new int[][] {new int[]{1,2,3}, new int[]{4,5,6}, new int[]{7,8,9}  });


        int[] arr = new int[] {3,1,45,6,4,0};

        for (int i = 0; i < arr.Length - 1; i++)
        {
            for (int j = 0; j < arr.Length - 1 - i; j++)
            {
                if (arr[j] > arr[j + 1])
                {
                    int temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                }
            }
        }

        foreach (var item in arr)
        {
            Debug.Log(item);
        }
    }


    public void Rotate(int[][] matrix) 
    {
        int n = matrix.Length;

        //将矩阵进行转置
        for (int i = 0; i < n; i++) 
        {
            for (int j = 0; j < i; j++) 
            {
                int temp = matrix[i][j];
                matrix[i][j] = matrix[j][i];
                matrix[j][i] = temp;
            }
        }

        //将矩阵的每一行反转
        for (int i = 0; i < n; i++) 
        {
            for (int j = 0, k = n - 1; j < k; j++, k--)
            {
                int temp = matrix[i][j];
                matrix[i][j] = matrix[i][k];
                matrix[i][k] = temp;
            }
        }
    }

}

public class RandomizedSet {
    private List<int> m_List = new List<int>();
    private Dictionary<int, int> m_Dictionary = new Dictionary<int,int>();

    private System.Random m_Random = new System.Random();


    public RandomizedSet() {

    }
    
    public bool Insert(int val) {
        if (m_Dictionary.ContainsKey(val)) return false;

        m_List.Add(val);
        m_Dictionary.Add(val, m_List.Count - 1);
        return true;
    }
    
    public bool Remove(int val) {
        if (!m_Dictionary.ContainsKey(val)) return false;

        int index = m_Dictionary[val];

        if (index == m_List.Count - 1)
        {
            m_List.RemoveAt(index);
        }
        else
        {
            int temp = m_List.Last();
            m_List[index] = temp;
            m_List[m_List.Count - 1] = val;
            m_Dictionary[temp] = index;

            m_List.RemoveAt(m_List.Count - 1);
        }

        m_Dictionary.Remove(val);

        return true;
    }
    
    public int GetRandom() {
        var order = m_Random.Next(0, m_List.Count);
        return m_List[order];
    }
}