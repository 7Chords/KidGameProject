using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using OfficeOpenXml;

public class ExcelImporterWindow : EditorWindow
{
    // 配置参数
    private string excelPath = "";
    private string assetOutputPath = "Assets/ExcelData";
    private string sheetName = "";
    private bool createSubfolders = true;
    private bool useFirstRowAsHeader = true;
    private Vector2 scrollPosition;

    // 导入结果
    private List<string> importLog = new List<string>();
    private int totalImported = 0;

    // 打开编辑器窗口
    [MenuItem("Tools/Excel 导入工具")]
    public static void ShowWindow()
    {
        GetWindow<ExcelImporterWindow>("Excel 导入工具");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Excel 导入配置", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Excel 文件路径
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Excel 文件", GUILayout.Width(100));
        excelPath = EditorGUILayout.TextField(excelPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFilePanel("选择 Excel 文件", "", "xlsx");
            if (!string.IsNullOrEmpty(path))
                excelPath = path;
        }
        EditorGUILayout.EndHorizontal();

        // 工作表名称
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("工作表名称", GUILayout.Width(100));
        sheetName = EditorGUILayout.TextField(sheetName);
        EditorGUILayout.EndHorizontal();

        // 资产输出路径
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("输出路径", GUILayout.Width(100));
        assetOutputPath = EditorGUILayout.TextField(assetOutputPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择输出路径", "Assets", "");
            if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
            {
                assetOutputPath = "Assets" + path.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();

        // 导入选项
        createSubfolders = EditorGUILayout.Toggle("按工作表创建子文件夹", createSubfolders);
        useFirstRowAsHeader = EditorGUILayout.Toggle("使用第一行为表头", useFirstRowAsHeader);

        // 导入按钮
        if (GUILayout.Button("导入 Excel 到 SO", GUILayout.Height(30)))
        {
            ImportExcel();
        }

        // 导入日志
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("导入日志", EditorStyles.boldLabel);
        foreach (string log in importLog)
        {
            EditorGUILayout.LabelField(log);
        }

        EditorGUILayout.EndScrollView();
    }

    private void ImportExcel()
    {
        importLog.Clear();
        totalImported = 0;

        if (string.IsNullOrEmpty(excelPath) || !File.Exists(excelPath))
        {
            importLog.Add("错误：Excel 文件不存在");
            return;
        }

        if (string.IsNullOrEmpty(assetOutputPath))
        {
            importLog.Add("错误：请设置输出路径");
            return;
        }

        try
        {
            // 初始化 EPPlus
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo fileInfo = new FileInfo(excelPath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                // 获取指定工作表或所有工作表
                List<ExcelWorksheet> worksheets = new List<ExcelWorksheet>();

                if (!string.IsNullOrEmpty(sheetName))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[sheetName];
                    if (ws == null)
                    {
                        importLog.Add($"错误：找不到名为 '{sheetName}' 的工作表");
                        return;
                    }
                    worksheets.Add(ws);
                }
                else
                {
                    // 如果未指定工作表，则导入所有工作表
                    foreach (ExcelWorksheet ws in package.Workbook.Worksheets)
                    {
                        worksheets.Add(ws);
                    }
                }

                // 处理每个工作表
                foreach (ExcelWorksheet ws in worksheets)
                {
                    ProcessWorksheet(ws);
                }
            }

            importLog.Add($"导入完成！共导入 {totalImported} 个资产");
            AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            importLog.Add($"致命错误：{e.Message}");
            Debug.LogError(e);
        }
    }

    private void ProcessWorksheet(ExcelWorksheet worksheet)
    {
        importLog.Add($"开始处理工作表: {worksheet.Name}");

        // 确定输出路径
        string outputPath = assetOutputPath;
        if (createSubfolders)
        {
            outputPath = Path.Combine(outputPath, worksheet.Name);
            if (!AssetDatabase.IsValidFolder(outputPath))
            {
                AssetDatabase.CreateFolder(assetOutputPath, worksheet.Name);
                importLog.Add($"创建文件夹: {outputPath}");
            }
        }

        // 读取表头（如果启用）
        List<string> headers = new List<string>();
        int startRow = 1;

        if (useFirstRowAsHeader)
        {
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                string header = worksheet.Cells[1, col].Text.Trim();
                headers.Add(string.IsNullOrEmpty(header) ? $"Column{col}" : header);
            }
            startRow = 2; // 数据从第2行开始
        }

        // 处理每一行数据
        for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
        {
            // 检查是否为空行
            bool isEmptyRow = true;
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                if (!string.IsNullOrEmpty(worksheet.Cells[row, col].Text))
                {
                    isEmptyRow = false;
                    break;
                }
            }

            if (isEmptyRow) continue;

            try
            {
                // 创建数据对象
                ExcelDataObject dataObject = ScriptableObject.CreateInstance<ExcelDataObject>();
                dataObject.Values = new Dictionary<string, string>();

                // 填充数据
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    string key = useFirstRowAsHeader ? headers[col - 1] : $"Column{col}";
                    string value = worksheet.Cells[row, col].Text;
                    dataObject.Values[key] = value;
                }

                // 生成资产名称
                string assetName = $"Row_{row}.asset";

                // 如果有ID列，使用ID作为资产名称
                if (dataObject.Values.ContainsKey("ID") && !string.IsNullOrEmpty(dataObject.Values["ID"]))
                {
                    assetName = $"{dataObject.Values["ID"]}.asset";
                }

                // 保存资产
                string assetPath = Path.Combine(outputPath, assetName);
                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                AssetDatabase.CreateAsset(dataObject, assetPath);

                totalImported++;
                importLog.Add($"已创建资产: {assetPath}");
            }
            catch (System.Exception e)
            {
                importLog.Add($"处理第 {row} 行时出错: {e.Message}");
            }
        }
    }
}