using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeValue
{
    [SerializeField] private float m_Base;
    [SerializeField] private float m_Origin;
    private bool m_IsInt = false;

    Dictionary<object, float> ADDDic = null;
    Dictionary<object, float> AULDic = null;
    Dictionary<object, float> MULDic = null;
    public AttributeValue(float value, bool int_flag = true)
    {
        m_Base  = value;
        m_Origin= value;
        m_IsInt = int_flag;
    }

    public void SetBase(float value)
    {
        m_Base = value;
    }

    public float GetBase()
    {
        return m_Base;
    }

    public float GetOrigin()
    {
        return m_Origin;
    }

    public void PutADD(object obj, float value)
    {
        if (ADDDic == null) ADDDic = new Dictionary<object, float>();

        if (ADDDic.ContainsKey(obj) == true) {
            ADDDic[obj] = value;
            return;
        }
        ADDDic.Add(obj, value);
    }

    public void PutAUL(object obj, float value)
    {
        if (AULDic == null) AULDic = new Dictionary<object, float>();

        if (AULDic.ContainsKey(obj) == true) {
            AULDic[obj] = value;
            return;
        }
        AULDic.Add(obj, value);
    }

    public void PutMUL(object obj, float value)
    {
        if (MULDic == null) MULDic = new Dictionary<object, float>();
        
        if (MULDic.ContainsKey(obj) == true) {
            MULDic[obj] = value;
            return;
        }
        MULDic.Add(obj, value);
    }

    public void Pop(object obj)
    {
        if (ADDDic != null && ADDDic.ContainsKey(obj) == true)
        {
            ADDDic.Remove(obj);
        }
        
        if (AULDic != null && AULDic.ContainsKey(obj) == true)
        {
            AULDic.Remove(obj);
        }

        if (MULDic != null && MULDic.ContainsKey(obj) == true)
        {
            MULDic.Remove(obj);
        }
    }

    public void Clear()
    {
        ADDDic?.Clear();
        AULDic?.Clear();
        MULDic?.Clear();
    }

    public float ToADDNumber()
    {
        var base_value  = m_Base;

        if (ADDDic != null) {
            foreach (var item in ADDDic) base_value += item.Value;
        }


        if (m_IsInt == true) {
            base_value = (float)Math.Floor(base_value);
        }

        return base_value;
    }

    public float ToNumber()
    {
        var base_value  = m_Base;

        var add_value   = 0f;
        var aul_value   = 1f;
        var mul_value   = 1f;

        if (ADDDic != null) {
            foreach (var value in ADDDic.Values) add_value += value;
        }

        if (AULDic != null) {
            foreach (var value in AULDic.Values) aul_value += value;
        }

        if (MULDic != null) {
            foreach (var value in MULDic.Values) mul_value *= value;
        }


        base_value = (base_value + add_value)  * aul_value * mul_value;

        if (m_IsInt == true) {
            base_value = (float)Math.Floor(base_value);
        }

        return base_value;
    }
}
