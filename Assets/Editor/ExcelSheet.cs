using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using OfficeOpenXml;

/// <summary>
/// Excel �ļ��洢�Ͷ�ȡ��
/// </summary>
public partial class ExcelSheet
{
    public static string SAVE_PATH = Application.streamingAssetsPath + "/Excel/";

    private int _rowCount = 0; // �������

    private int _colCount = 0; // �������

    public int RowCount { get => _rowCount; }

    public int ColCount { get => _colCount; }

    private Dictionary<Index, string> _sheetDic = new Dictionary<Index, string>(); // ���浱ǰ���ݵ��ֵ�

    public ExcelSheet() { }

    public ExcelSheet(string filePath, string sheetName = null, FileFormat format = FileFormat.Xlsx)
    {
        Load(filePath, sheetName, format);
    }

    public string this[int row, int col]
    {
        get
        {
            // Խ����
            if (row >= _rowCount || row < 0)
                Debug.LogError($"ExcelSheet: Row {row} out of range!");
            if (col >= _colCount || col < 0)
                Debug.LogError($"ExcelSheet: Column {col} out of range!");

            // �����ڽ�����򷵻ؿ��ַ���
            return _sheetDic.GetValueOrDefault(new Index(row, col), "");
        }
        set
        {
            _sheetDic[new Index(row, col)] = value;

            // ��¼�������������
            if (row >= _rowCount) _rowCount = row + 1;
            if (col >= _colCount) _colCount = col + 1;
        }
    }

    /// <summary>
    /// �洢 Excel �ļ�
    /// </summary>
    /// <param name="filePath">�ļ�·��������Ҫд�ļ���չ��</param>
    /// <param name="sheetName">���������û��ָ����������ʹ���ļ�������ʹ�� csv ��ʽ������Դ˲���</param>
    /// <param name="format">������ļ���ʽ</param>
    public void Save(string filePath, string sheetName = null, FileFormat format = FileFormat.Xlsx)
    {
        string fullPath = SAVE_PATH + filePath + FileFormatToExtension(format); // �ļ�����·��
        var index = fullPath.LastIndexOf("/", StringComparison.Ordinal);
        var directory = fullPath[..index];

        if (!Directory.Exists(directory))
        { // ����ļ����ڵ�Ŀ¼�����ڣ����ȴ���Ŀ¼
            Directory.CreateDirectory(directory);
        }

        switch (format)
        {
            case FileFormat.Xlsx:
                SaveAsXlsx(fullPath, sheetName);
                break;
            case FileFormat.Csv:
                SaveAsCsv(fullPath);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }

        Debug.Log($"ExcelSheet: Save sheet \"{filePath}::{sheetName}\" successfully.");
    }

    /// <summary>
    /// ��ȡ Excel �ļ�
    /// </summary>
    /// <param name="filePath">�ļ�·��������Ҫд�ļ���չ��</param>
    /// <param name="sheetName">���������û��ָ����������ʹ���ļ���</param>
    /// <param name="format">������ļ���ʽ</param>
    public void Load(string filePath, string sheetName = null, FileFormat format = FileFormat.Xlsx)
    {
        // ��յ�ǰ����
        Clear();
        string fullPath = SAVE_PATH + filePath + FileFormatToExtension(format); // �ļ�����·��

        if (!File.Exists(fullPath))
        { // �������ļ����򱨴�
            Debug.LogError($"ExcelSheet: Can't find path \"{fullPath}\".");
            return;
        }

        switch (format)
        {
            case FileFormat.Xlsx:
                LoadFromXlsx(fullPath, sheetName);
                break;
            case FileFormat.Csv:
                LoadFromCsv(fullPath);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }

        Debug.Log($"ExcelSheet: Load sheet \"{filePath}::{sheetName}\" successfully.");
    }

    public void Clear()
    {
        _sheetDic.Clear();
        _rowCount = 0;
        _colCount = 0;
    }
}

public partial class ExcelSheet
{
    public struct Index
    {
        public int Row;
        public int Col;

        public Index(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }

    /// <summary>
    /// ������ļ���ʽ
    /// </summary>
    public enum FileFormat
    {
        Xlsx,
        Csv
    }

    private string FileFormatToExtension(FileFormat format)
    {
        return $".{format.ToString().ToLower()}";
    }

    private void SaveAsXlsx(string fullPath, string sheetName)
    {
        var index = fullPath.LastIndexOf("/", StringComparison.Ordinal);
        var fileName = fullPath[(index + 1)..];
        sheetName ??= fileName[..fileName.IndexOf(".", StringComparison.Ordinal)]; // ���û��ָ����������ʹ���ļ���

        var fileInfo = new FileInfo(fullPath);
        using var package = new ExcelPackage(fileInfo);

        if (!File.Exists(fullPath) ||                         // ������ Excel
            package.Workbook.Worksheets[sheetName] == null)
        { // ����û�б�����ӱ�
            package.Workbook.Worksheets.Add(sheetName);       // ������ʱ��Excel �ļ�Ҳ�ᱻ����
        }

        var sheet = package.Workbook.Worksheets[sheetName];

        var cells = sheet.Cells;
        cells.Clear(); // ���������

        foreach (var pair in _sheetDic)
        {
            var i = pair.Key.Row;
            var j = pair.Key.Col;
            cells[i + 1, j + 1].Value = pair.Value;
        }

        package.Save(); // �����ļ�
    }

    private void SaveAsCsv(string fullPath)
    {
        using FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);

        Index idx = new Index(0, 0);
        for (int i = 0; i < _rowCount; i++)
        {
            idx.Row = i;
            idx.Col = 0;

            // д���һ�� value
            var value = _sheetDic.GetValueOrDefault(idx, "");
            if (!string.IsNullOrEmpty(value))
                fs.Write(Encoding.UTF8.GetBytes(value));

            // д����� value����Ҫ��� ","
            for (int j = 1; j < _colCount; j++)
            {
                idx.Col = j;
                value = "," + _sheetDic.GetValueOrDefault(idx, "");
                fs.Write(Encoding.UTF8.GetBytes(value));
            }

            // д�� "\n"
            fs.Write(Encoding.UTF8.GetBytes("\n"));
        }
    }

    private void LoadFromXlsx(string fullPath, string sheetName)
    {
        var index = fullPath.LastIndexOf("/", StringComparison.Ordinal);
        var fileName = fullPath[(index + 1)..];
        sheetName ??= fileName[..fileName.IndexOf(".", StringComparison.Ordinal)]; // ���û��ָ����������ʹ���ļ���

        var fileInfo = new FileInfo(fullPath);

        using var package = new ExcelPackage(fileInfo);

        var sheet = package.Workbook.Worksheets[sheetName];

        if (sheet == null)
        { // �����ڱ��򱨴�
            Debug.LogError($"ExcelSheet: Can't find sheet \"{sheetName}\" in file \"{fullPath}\"");
            return;
        }

        _rowCount = sheet.Dimension.Rows;
        _colCount = sheet.Dimension.Columns;

        var cells = sheet.Cells;
        for (int i = 0; i < _rowCount; i++)
        {
            for (int j = 0; j < _colCount; j++)
            {
                var value = cells[i + 1, j + 1].Text;
                if (string.IsNullOrEmpty(value)) continue; // �����ݲż�¼
                _sheetDic.Add(new Index(i, j), value);
            }
        }
    }

    private void LoadFromCsv(string fullPath)
    {
        // ��ȡ�ļ�
        string[] lines = File.ReadAllLines(fullPath); // ��ȡ������
        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split(','); // ��ȡһ�У����ŷָ�
            for (int j = 0; j < line.Length; j++)
            {
                if (line[j] != "") // �����ݲż�¼
                    _sheetDic.Add(new Index(i, j), line[j]);
            }

            // �����������������
            _colCount = Mathf.Max(_colCount, line.Length);
            _rowCount = i + 1;
        }
    }
}
