using UnityEngine;
 
public class CSVUtility : MonoBehaviour
{
    public TextAsset csvFile; // 通过 Unity 编辑器将 CSV 文件分配给这个变量
 
    void Start()
    {
        if (csvFile != null)
        {
            string[] lines = csvFile.text.Split('\n'); // 将 CSV 文件的内容分割为行
 
            foreach (string line in lines)
            {
                string[] fields = line.Split(','); // 根据逗号分隔每一行的字段
                
                // 处理每个字段的数据...
                // 在这里，你可以访问 fields 数组中的每个元素，以获取每行数据的每个字段
                
                // 举例：输出每行数据的第一个字段
                if (fields.Length > 0)
                {
                    Debug.Log("Length : " + fields.Length);
                    string firstField = fields[0];
                    Debug.Log("1 of the line: " + fields[0]);
                    Debug.Log("2 of the line: " + fields[1]);
                    Debug.Log("3 of the line: " + fields[2]);
                    Debug.Log("4 of the line: " + fields[3]);
                    Debug.Log("5 of the line: " + fields[4]);
                }
            }
        }
        else
        {
            Debug.LogError("CSV file is not assigned!");
        }
    }
}