using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using OfficeOpenXml;

public class ExcelImporterWindow : EditorWindow
{
    // ���ò���
    private string excelPath = "";
    private string assetOutputPath = "Assets/ExcelData";
    private string sheetName = "";
    private bool createSubfolders = true;
    private bool useFirstRowAsHeader = true;
    private Vector2 scrollPosition;

    // ������
    private List<string> importLog = new List<string>();
    private int totalImported = 0;

    // �򿪱༭������
    [MenuItem("Tools/Excel ���빤��")]
    public static void ShowWindow()
    {
        GetWindow<ExcelImporterWindow>("Excel ���빤��");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Excel ��������", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Excel �ļ�·��
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Excel �ļ�", GUILayout.Width(100));
        excelPath = EditorGUILayout.TextField(excelPath);
        if (GUILayout.Button("���", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFilePanel("ѡ�� Excel �ļ�", "", "xlsx");
            if (!string.IsNullOrEmpty(path))
                excelPath = path;
        }
        EditorGUILayout.EndHorizontal();

        // ����������
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("����������", GUILayout.Width(100));
        sheetName = EditorGUILayout.TextField(sheetName);
        EditorGUILayout.EndHorizontal();

        // �ʲ����·��
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("���·��", GUILayout.Width(100));
        assetOutputPath = EditorGUILayout.TextField(assetOutputPath);
        if (GUILayout.Button("���", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("ѡ�����·��", "Assets", "");
            if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
            {
                assetOutputPath = "Assets" + path.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();

        // ����ѡ��
        createSubfolders = EditorGUILayout.Toggle("�������������ļ���", createSubfolders);
        useFirstRowAsHeader = EditorGUILayout.Toggle("ʹ�õ�һ��Ϊ��ͷ", useFirstRowAsHeader);

        // ���밴ť
        if (GUILayout.Button("���� Excel �� SO", GUILayout.Height(30)))
        {
            ImportExcel();
        }

        // ������־
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("������־", EditorStyles.boldLabel);
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
            importLog.Add("����Excel �ļ�������");
            return;
        }

        if (string.IsNullOrEmpty(assetOutputPath))
        {
            importLog.Add("�������������·��");
            return;
        }

        try
        {
            // ��ʼ�� EPPlus
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo fileInfo = new FileInfo(excelPath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                // ��ȡָ������������й�����
                List<ExcelWorksheet> worksheets = new List<ExcelWorksheet>();

                if (!string.IsNullOrEmpty(sheetName))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[sheetName];
                    if (ws == null)
                    {
                        importLog.Add($"�����Ҳ�����Ϊ '{sheetName}' �Ĺ�����");
                        return;
                    }
                    worksheets.Add(ws);
                }
                else
                {
                    // ���δָ���������������й�����
                    foreach (ExcelWorksheet ws in package.Workbook.Worksheets)
                    {
                        worksheets.Add(ws);
                    }
                }

                // ����ÿ��������
                foreach (ExcelWorksheet ws in worksheets)
                {
                    ProcessWorksheet(ws);
                }
            }

            importLog.Add($"������ɣ������� {totalImported} ���ʲ�");
            AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            importLog.Add($"��������{e.Message}");
            Debug.LogError(e);
        }
    }

    private void ProcessWorksheet(ExcelWorksheet worksheet)
    {
        importLog.Add($"��ʼ��������: {worksheet.Name}");

        // ȷ�����·��
        string outputPath = assetOutputPath;
        if (createSubfolders)
        {
            outputPath = Path.Combine(outputPath, worksheet.Name);
            if (!AssetDatabase.IsValidFolder(outputPath))
            {
                AssetDatabase.CreateFolder(assetOutputPath, worksheet.Name);
                importLog.Add($"�����ļ���: {outputPath}");
            }
        }

        // ��ȡ��ͷ��������ã�
        List<string> headers = new List<string>();
        int startRow = 1;

        if (useFirstRowAsHeader)
        {
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                string header = worksheet.Cells[1, col].Text.Trim();
                headers.Add(string.IsNullOrEmpty(header) ? $"Column{col}" : header);
            }
            startRow = 2; // ���ݴӵ�2�п�ʼ
        }

        // ����ÿһ������
        for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
        {
            // ����Ƿ�Ϊ����
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
                // �������ݶ���
                ExcelDataObject dataObject = ScriptableObject.CreateInstance<ExcelDataObject>();
                dataObject.Values = new Dictionary<string, string>();

                // �������
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    string key = useFirstRowAsHeader ? headers[col - 1] : $"Column{col}";
                    string value = worksheet.Cells[row, col].Text;
                    dataObject.Values[key] = value;
                }

                // �����ʲ�����
                string assetName = $"Row_{row}.asset";

                // �����ID�У�ʹ��ID��Ϊ�ʲ�����
                if (dataObject.Values.ContainsKey("ID") && !string.IsNullOrEmpty(dataObject.Values["ID"]))
                {
                    assetName = $"{dataObject.Values["ID"]}.asset";
                }

                // �����ʲ�
                string assetPath = Path.Combine(outputPath, assetName);
                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                AssetDatabase.CreateAsset(dataObject, assetPath);

                totalImported++;
                importLog.Add($"�Ѵ����ʲ�: {assetPath}");
            }
            catch (System.Exception e)
            {
                importLog.Add($"����� {row} ��ʱ����: {e.Message}");
            }
        }
    }
}