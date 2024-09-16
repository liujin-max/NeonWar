using System.IO;
using UnityEditor;
using UnityEngine;

public class ScriptGenerator : EditorWindow
{
    private string className = "NewClass";



    [MenuItem("自定义工具/自动创建MVC脚本")]
    public static void ShowWindow()
    {
        GetWindow<ScriptGenerator>("Script Generator");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("创建MVC脚本", EditorStyles.boldLabel);

        GUILayout.Space(10);
        className = EditorGUILayout.TextField("Window名称", className);

        GUILayout.Space(20);

        if (GUILayout.Button("生成"))
        {
            //创建Window脚本
            CreateScript("Assets/Editor/ScriptTemplates/UIController.cs.txt", $"Assets/Scripts/UI/Controller/UICtrl_{className}.cs", className);

            //创建Controller脚本
            CreateScript("Assets/Editor/ScriptTemplates/UIWindow.cs.txt", $"Assets/Scripts/UI/Window/{className}.cs", className);
        }
    }

    void CreateScript(string templatePath, string scriptPath, string newClassName)
    {
        if (!File.Exists(templatePath))
        {
            Debug.LogError("未找到模板文件：" + templatePath);
            return; 
        }

        // 读取模版文件内容
        string templateContent = File.ReadAllText(templatePath);

        // 将占位符 #SCRIPTNAME# 替换为实际的类名
        string scriptContent = templateContent.Replace("#SCRIPTNAME#", newClassName);

        // 确保脚本目录存在
        Directory.CreateDirectory(Path.GetDirectoryName(scriptPath));

        // 写入新的脚本文件
        File.WriteAllText(scriptPath, scriptContent);

        // 刷新 Unity 的 Asset 数据库，确保新脚本出现在项目视图中
        AssetDatabase.Refresh();

        Debug.Log($"Script {newClassName} created at {scriptPath}");
    }
}
