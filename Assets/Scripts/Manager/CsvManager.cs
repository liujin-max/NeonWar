using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
 



public class CsvManager: MonoBehaviour
{
    //表名
    public const string TableKey_Skill   = "Skill";



    //表名与表结构
    private Dictionary<string, List<string[]>> m_DataLists = new Dictionary<string, List<string[]>>();
    private Dictionary<string, Dictionary<int, string[]>> m_DataDics = new Dictionary<string, Dictionary<int, string[]>>();



    public void ReadCsvs()
    {
        this.ReadCsv(CsvManager.TableKey_Skill,          "CSV/Skill");
    }

    public string[] GetStringArray(string excel_name, int id)
    {
        Dictionary<int, string[]> dic;
        if (m_DataDics.TryGetValue(excel_name, out dic)) {
            string[] data;
            if (dic.TryGetValue(id, out data)) {
                return data;
            }
        }

        Debug.LogError(excel_name + " is nil : " + id);
        return null;
    }
    

    public List<string[]> GetStringArrays(string excel_name)
    {
        List<string[]> list;
        if (m_DataLists.TryGetValue(excel_name, out list)) {
            return list;
        }

        return null;
    }

    public void ReadCsv(string table_key, string csvName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(csvName);

        if (csvFile == null)  {
            Debug.LogError("CSV file is not assigned! : " + csvName);
            return;
        }


        List<string[]> data_list = new List<string[]>();
        m_DataLists.Add(table_key, data_list);

        Dictionary<int, string[]> dic = new Dictionary<int, string[]>();
        m_DataDics.Add(table_key, dic);

        string[] lines = csvFile.text.Split('\n'); // 将 CSV 文件的内容分割为行

        //从第二行开始
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];

            string[] fields = line.Split(','); // 根据逗号分隔每一行的字段
            
            // 处理每个字段的数据...
            // 在这里，你可以访问 fields 数组中的每个元素，以获取每行数据的每个字段
            // 举例：输出每行数据的第一个字段
            if (fields.Length > 0 && string.IsNullOrEmpty(fields[0]) != true)
            {
                // Debug.Log(fields[0]);
                int id = Convert.ToInt32(fields[0]);

                data_list.Add(fields);
                dic.Add(id, fields);

            }
        }
    }

}































