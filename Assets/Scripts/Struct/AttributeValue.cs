using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Collections.Generic.Dictionary<object, float>;


[System.Serializable]
public class AttributeValue
{
    [SerializeField] private float m_Base;
    private bool m_IsInt;

    Dictionary<object, float> ADDDic;
    Dictionary<object, float> AULDic;
    Dictionary<object, float> MULDic;

    ValueCollection ADDValues;
    ValueCollection AULValues;
    ValueCollection MULValues;


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
        if (ADDDic == null) {
            ADDDic = new Dictionary<object, float>();
            ADDValues = ADDDic.Values;
        }

        if (ADDDic.ContainsKey(obj) == true) {
            ADDDic[obj] = value;
            return;
        }
        ADDDic.Add(obj, value);
    }

    public void PutAUL(object obj, float value)
    {
        if (AULDic == null) {
            AULDic = new Dictionary<object, float>();
            AULValues = AULDic.Values;
        }

        if (AULDic.ContainsKey(obj) == true) {
            AULDic[obj] = value;
            return;
        }
        AULDic.Add(obj, value);
    }

    public void PutMUL(object obj, float value)
    {
        if (MULDic == null) {
            MULDic = new Dictionary<object, float>();
            MULValues = MULDic.Values;
        }

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

    public float ToNumber(bool is_floor = true)
    {
        var base_value  = m_Base;

        var add_value   = 0f;
        var aul_value   = 1f;
        var mul_value   = 1f;

        if (ADDDic != null) {
            foreach (var value in ADDValues) add_value += value;
        }

        if (AULDic != null) {
            foreach (var value in AULValues) aul_value += value;
        }

        if (MULDic != null) {
            foreach (var value in MULValues) mul_value *= value;
        }


        base_value = (base_value + add_value)  * aul_value * mul_value;

        if (m_IsInt == true) {
            if (is_floor) base_value = (float)Mathf.Floor(base_value);
            else base_value = (float)Mathf.Ceil(base_value);
        }

        return base_value;
    }
}
