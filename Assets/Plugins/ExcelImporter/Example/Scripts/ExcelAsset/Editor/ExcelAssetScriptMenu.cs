using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class ExcelAssetScriptMenu : EditorWindow
{
	const string ScriptTemplateName = "ExcelAssetScriptTemplete.cs.txt";
	const string FieldTemplete = "\t//public List<EntityType> #FIELDNAME#; // Replace 'EntityType' to an actual type that is serializable.";
    string relativePath;//相对路径
    private string selectedExcelName = "未选择文件";
    private string selectedExcelPath = "";
    [MenuItem("自定义编辑器/Excel导出")]
    public static void ShowWindow()
    {
        GetWindow<ExcelAssetScriptMenu>("Excel导出");
    }
    void OnGUI()
    {
        // 文件选择按钮
        GUILayout.Label("请先确保你的Excel导入了Unity文件夹中！！！", new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            normal = { textColor = Color.white }
        });
        GUILayout.Label("该功能仅支持Asset内的Excel表！！！", new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            normal = { textColor = Color.white }
        });
        GUILayout.Label("Excel表建议放在Refdata/Excels下", new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            normal = { textColor = Color.white }
        });
        EditorGUILayout.LabelField("当前选择文件路径:", selectedExcelName);
        if (GUILayout.Button("选择Excel文件", GUILayout.Width(222)))
        {
            AbsolutePath2RelativePath();
        }
        // 选择路径并且生成脚本
        GUILayout.Label("选择路径后会直接生成脚本！！！", new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12,
            normal = { textColor = Color.red }
        });
        GUILayout.Label("Script建议都放在该目录下方便管理↓↓", EditorStyles.boldLabel);
        GUILayout.Label("Asset/RefData/Scripts/ExcelAsset", EditorStyles.boldLabel);
        //
        if (GUILayout.Button("选择脚本生成路径", GUILayout.Width(222)))
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                EditorUtility.DisplayDialog("错误", "请先选择Excel文件", "确定");
                return;
            }
            CreateScript(relativePath);
        }
        GUILayout.Label("请前往脚本修改字段类型", EditorStyles.boldLabel);
        GUILayout.Label("1.确保工作表(sheet)名与脚本字段名一致！！！", EditorStyles.boldLabel);
        GUILayout.Label("2.确保脚本List<Entity> Entity数据类型 与 工作表第一行一致！！！", EditorStyles.boldLabel);
        GUILayout.Label("----------------------------------------------------------", EditorStyles.boldLabel);
        GUILayout.Label("↓如果so没有正常生成或者更改 请尝试这个按钮↓", EditorStyles.boldLabel);
        if (GUILayout.Button("重新导入这个Excel", GUILayout.Width(222)))
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                EditorUtility.DisplayDialog("错误", "请先选择Excel文件", "确定");
                return;
            }
            ReimportSelectedAsset(relativePath);
        }
        GUILayout.Label("----------------------------------------------------------", EditorStyles.boldLabel);

        GUILayout.Label("说明：", new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            normal = { textColor = Color.white }
        });
        GUILayout.Label("1.So会自动生成在Excel表同目录下", EditorStyles.boldLabel);
        GUILayout.Label("2.Entity的定义建议都放在该目录下↓↓", EditorStyles.boldLabel);
        GUILayout.Label("Asset/RefData/Scripts/Entity", EditorStyles.boldLabel);
    }

    public static void ReimportSelectedAsset(string assetPath)
    {

        if (!string.IsNullOrEmpty(assetPath))
        {
            // 重新导入单个资源
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            EditorUtility.DisplayDialog("Reimport", "已重新导入", "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "没有选中任何有效资源", "确定");
        }
    }

    private void AbsolutePath2RelativePath()
    {
        //设置打开的默认路径
        string defaultPath = Path.Combine(Application.dataPath, "Assets/RefData/Excels");
        //获取Excel的绝对路径
        string absolutePath = EditorUtility.OpenFilePanel("选择一个Excel表", defaultPath, "xlsx,xls");
        if (string.IsNullOrEmpty(absolutePath))
        {
            Debug.Log("未选择文件！");
            return;
        }
        // 转化为相对路径
        // 例如： absolutePath = "C:\\MyUnityProject\\Assets\\RefData\\Excels\\Data.xlsx";
        // \\ -> /
        // C:/MyUnityProject/Assets/RefData/Excels/Data.xlsx
        // Application.dataPath 得到的是C:/MyUnityProject/Assets 所以给他替换成 空 
        // /RefData/Excels/Data.xlsx
        relativePath = "Assets" + absolutePath.Replace('\\', '/').Replace(Application.dataPath, "");
        selectedExcelName = relativePath;
        AssetDatabase.Refresh();
    }
    static void CreateScript(string excelPath)
    {
        //设置打开的默认路径
        string defaultPath = Path.Combine(Application.dataPath, "RefData/Scripts/ExcelAsset");
        string savePath = EditorUtility.SaveFolderPanel("保存脚本", defaultPath, "");
        if (string.IsNullOrEmpty(savePath)) return;

        string excelName = Path.GetFileNameWithoutExtension(excelPath);
        List<string> sheetNames = GetSheetNames(excelPath);

        string scriptString = BuildScriptString(excelName, sheetNames);

        string path = Path.ChangeExtension(Path.Combine(savePath, excelName), "cs");
        File.WriteAllText(path, scriptString);

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("完成", $"脚本已生成在：{path}", "确定");
    }
    static bool CreateScriptValidation()
	{
		var selectedAssets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
		if(selectedAssets.Length != 1) return false;
		var path = AssetDatabase.GetAssetPath(selectedAssets[0]);
		return Path.GetExtension(path) == ".xls" || Path.GetExtension(path) == ".xlsx";
	}

	static List<string> GetSheetNames(string excelPath)
	{
		var sheetNames = new List<string>();
		using(FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			IWorkbook book = null;
			if (Path.GetExtension(excelPath) == ".xls") book = new HSSFWorkbook(stream);
			else book = new XSSFWorkbook(stream);

			for(int i = 0; i < book.NumberOfSheets; i++)
			{
				var sheet = book.GetSheetAt(i);
				sheetNames.Add(sheet.SheetName);
			}
		}
		return sheetNames;
	}

	static string GetScriptTempleteString()
	{
		string currentDirectory = Directory.GetCurrentDirectory();
		string[] filePath = Directory.GetFiles(currentDirectory, ScriptTemplateName, SearchOption.AllDirectories);
		if(filePath.Length == 0) throw new Exception("Script template not found.");

		string templateString = File.ReadAllText(filePath[0]);
		return templateString;
	}

	static string BuildScriptString(string excelName, List<string> sheetNames)
	{
		string scriptString = GetScriptTempleteString();

		scriptString = scriptString.Replace("#ASSETSCRIPTNAME#", excelName);

		foreach(string sheetName in sheetNames)
		{
			string fieldString = String.Copy(FieldTemplete);
			fieldString = fieldString.Replace("#FIELDNAME#", sheetName);
			fieldString += "\n#ENTITYFIELDS#";
			scriptString = scriptString.Replace("#ENTITYFIELDS#", fieldString);
		}
		scriptString = scriptString.Replace("#ENTITYFIELDS#\n", "");

		return scriptString;
	}
}