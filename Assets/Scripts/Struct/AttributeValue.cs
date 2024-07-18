using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeValue
{
    private float m_Base;
    private bool m_IsInt = false;

    Dictionary<object, float> ADDDic = new Dictionary<object, float>();
    Dictionary<object, float> AULDic = new Dictionary<object, float>();
    Dictionary<object, float> MULDic = new Dictionary<object, float>();
    public AttributeValue(float value, bool int_flag = true)
    {
        m_Base  = value;
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

    public void PutADD(object obj, float value)
    {
        if (ADDDic.ContainsKey(obj) == true) {
            ADDDic[obj] = value;
            return;
        }
        ADDDic.Add(obj, value);
    }

    public void PutAUL(object obj, float value)
    {
        if (AULDic.ContainsKey(obj) == true) {
            AULDic[obj] = value;
            return;
        }
        AULDic.Add(obj, value);
    }

    public void PutMUL(object obj, float value)
    {
        if (MULDic.ContainsKey(obj) == true) {
            MULDic[obj] = value;
            return;
        }
        MULDic.Add(obj, value);
    }

    public void Pop(object obj)
    {
        if (ADDDic.ContainsKey(obj) == true)
        {
            ADDDic.Remove(obj);
        }
        
        if (AULDic.ContainsKey(obj) == true)
        {
            AULDic.Remove(obj);
        }

        if (MULDic.ContainsKey(obj) == true)
        {
            MULDic.Remove(obj);
        }
    }

    public float ShowRate()
    {
        var aul_value   = 1f;
        var mul_value   = 1f;

        foreach (var item in AULDic) {
            aul_value += item.Value;
        }

        foreach (var item in MULDic) {
            mul_value *= item.Value;
        }

        var rate = aul_value * mul_value;
        var base_value = (float)Math.Round(rate, 1);

        return base_value;
    }

    public void Clear()
    {
        ADDDic.Clear();
        AULDic.Clear();
        MULDic.Clear();
    }

    public float ToADDNumber()
    {
        var base_value  = m_Base;

        foreach (var item in ADDDic) {
            base_value += item.Value;
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


        foreach (var item in ADDDic) {
            add_value += item.Value;
        }


        foreach (var item in AULDic) {
            aul_value += item.Value;
        }

        foreach (var item in MULDic) {
            mul_value *= item.Value;
        }

        base_value = (base_value + add_value)  * aul_value * mul_value;

        if (m_IsInt == true) {
            base_value = (float)Math.Floor(base_value);
        }

        return base_value;
    }
}
