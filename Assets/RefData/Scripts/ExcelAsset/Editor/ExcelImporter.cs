﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;




public class ExcelImporter : AssetPostprocessor
{

    class ExcelAssetInfo
    {
        public Type AssetType { get; set; }
        public ExcelAssetAttribute Attribute { get; set; }
        public string ExcelName
        {
            get
            {
                return string.IsNullOrEmpty(Attribute.ExcelName) ? AssetType.Name : Attribute.ExcelName;
            }
        }
    }

    static List<ExcelAssetInfo> cachedInfos = null; // Clear on compile.

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        bool imported = false;
        foreach (string path in importedAssets)
        {
            if (Path.GetExtension(path) == ".xls" || Path.GetExtension(path) == ".xlsx")
            {
                if (cachedInfos == null) cachedInfos = FindExcelAssetInfos();

                var excelName = Path.GetFileNameWithoutExtension(path);
                if (excelName.StartsWith("~$")) continue;

                ExcelAssetInfo info = cachedInfos.Find(i => i.ExcelName == excelName);

                if (info == null) continue;

                ImportExcel(path, info);
                imported = true;
            }
        }

        if (imported)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    static List<ExcelAssetInfo> FindExcelAssetInfos()
    {
        var list = new List<ExcelAssetInfo>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                var attributes = type.GetCustomAttributes(typeof(ExcelAssetAttribute), false);
                if (attributes.Length == 0) continue;
                var attribute = (ExcelAssetAttribute)attributes[0];
                var info = new ExcelAssetInfo()
                {
                    AssetType = type,
                    Attribute = attribute
                };
                list.Add(info);
            }
        }
        return list;
    }

    static UnityEngine.Object LoadOrCreateAsset(string assetPath, Type assetType)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(assetPath));

        var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance(assetType.Name);
            AssetDatabase.CreateAsset((ScriptableObject)asset, assetPath);
            asset.hideFlags = HideFlags.None;
        }

        return asset;
    }

    static IWorkbook LoadBook(string excelPath)
    {
        using (FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            if (Path.GetExtension(excelPath) == ".xls") return new HSSFWorkbook(stream);
            else return new XSSFWorkbook(stream);
        }
    }

    static List<string> GetFieldNamesFromSheetHeader(ISheet sheet)
    {
        IRow headerRow = sheet.GetRow(0);

        var fieldNames = new List<string>();
        for (int i = 0; i < headerRow.LastCellNum; i++)
        {
            var cell = headerRow.GetCell(i);
            if (cell == null || cell.CellType == CellType.Blank) break;
            fieldNames.Add(cell.StringCellValue);
        }
        return fieldNames;
    }

    static object CellToFieldObject(ICell cell, FieldInfo fieldInfo, bool isFormulaEvalute = false)
    {
        var type = isFormulaEvalute ? cell.CachedFormulaResultType : cell.CellType;

        switch (type)
        {
            case CellType.String:
                if (fieldInfo.FieldType.IsEnum) return Enum.Parse(fieldInfo.FieldType, cell.StringCellValue);
                else return cell.StringCellValue;
            case CellType.Boolean:
                return cell.BooleanCellValue;
            case CellType.Numeric:
                return Convert.ChangeType(cell.NumericCellValue, fieldInfo.FieldType);
            case CellType.Formula:
                if (isFormulaEvalute) return null;
                return CellToFieldObject(cell, fieldInfo, true);
            default:
                if (fieldInfo.FieldType.IsValueType)
                {
                    return Activator.CreateInstance(fieldInfo.FieldType);
                }
                return null;
        }
    }

    // 新增方法：处理列表类型的单元格
    static object CellToListFieldObject(ICell cell, FieldInfo fieldInfo, bool isFormulaEvaluate = false)
    {
        var listType = fieldInfo.FieldType;

        Type elementType = listType.GetGenericArguments()[0];
        // 创建List实例
        object list = Activator.CreateInstance(listType);
        MethodInfo addMethod = listType.GetMethod("Add");

        // 如果单元格是空的，返回空列表
        if (cell == null || cell.CellType == CellType.Blank)
        {
            return list;
        }

        // 分割单元格内容
        string[] values = cell.StringCellValue.Split(';');
        foreach (string value in values)
        {
            // 去除首位的空白部分
            string trimmedValue = value.Trim();
            if (!string.IsNullOrEmpty(trimmedValue))
            {
                object elementValue;
                try
                {
                    // 特殊处理枚举类型
                    if (elementType.IsEnum)
                    {
                        // 尝试通过名称转换枚举
                        elementValue = Enum.Parse(elementType, trimmedValue);
                    }
                    else
                    {
                        // 普通类型使用Convert.ChangeType
                        elementValue = Convert.ChangeType(trimmedValue, elementType);
                    }
                    addMethod.Invoke(list, new object[] { elementValue });
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to convert value '{trimmedValue}' to type {elementType.Name}: {ex.Message}");
                }
            }
        }

        return list;
    }

    /*static object CreateEntityFromRow(IRow row, List<string> columnNames, Type entityType, string sheetName)
    {
        var entity = Activator.CreateInstance(entityType);

        for (int i = 0; i < columnNames.Count; i++)
        {
            FieldInfo entityField = entityType.GetField(
                columnNames[i],
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic 
            );
            if (entityField == null) continue;
            if (!entityField.IsPublic && entityField.GetCustomAttributes(typeof(SerializeField), false).Length == 0) continue;

            ICell cell = row.GetCell(i);
            if (cell == null) continue;

            try
            {
                object fieldValue = CellToFieldObject(cell, entityField);
                entityField.SetValue(entity, fieldValue);
            }
            catch
            {
                throw new Exception(string.Format("Invalid excel cell type at row {0}, column {1}, {2} sheet.", row.RowNum, cell.ColumnIndex, sheetName));
            }
        }
        return entity;
    }*/

    static object CreateEntityFromRow(IRow row, List<string> columnNames, Type entityType, string sheetName)
    {
        object entity;
        // 判断实体类型是否是 ScriptableObject 的子类
        if (typeof(ScriptableObject).IsAssignableFrom(entityType))
        {
            // 对于 ScriptableObject 子类，必须用 CreateInstance 实例化
            entity = ScriptableObject.CreateInstance(entityType);
        }
        else
        {
            // 普通类仍用 Activator.CreateInstance
            entity = Activator.CreateInstance(entityType);
        }

        // 后续字段赋值逻辑不变...
        for (int i = 0; i < columnNames.Count; i++)
        {
            FieldInfo entityField = entityType.GetField(
                columnNames[i],
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (entityField == null) continue;
            if (!entityField.IsPublic && entityField.GetCustomAttributes(typeof(SerializeField), false).Length == 0) continue;

            ICell cell = row.GetCell(i);
            if (cell == null) continue;

            try
            {
                object fieldValue;

                if (entityField.FieldType.IsGenericType &&
                    entityField.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    fieldValue = CellToListFieldObject(cell, entityField);
                }
                else
                {
                    fieldValue = CellToFieldObject(cell, entityField);
                }

                entityField.SetValue(entity, fieldValue);
            }
            catch (Exception ex)
            {
                throw new Exception($"处理行 {row.RowNum} 列 {i} 时出错：{ex.Message}", ex);
            }
        }
        return entity;
    }

    static object GetEntityListFromSheet(ISheet sheet, Type entityType)
    {
        List<string> excelColumnNames = GetFieldNamesFromSheetHeader(sheet);

        Type listType = typeof(List<>).MakeGenericType(entityType);
        MethodInfo listAddMethod = listType.GetMethod("Add", new Type[] { entityType });
        object list = Activator.CreateInstance(listType);

        // row of index 0 is header
        for (int i = 1; i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            if (row == null) break;

            ICell entryCell = row.GetCell(0);
            if (entryCell == null || entryCell.CellType == CellType.Blank) break;

            // skip comment row
            if (entryCell.CellType == CellType.String && entryCell.StringCellValue.StartsWith("#")) continue;

            var entity = CreateEntityFromRow(row, excelColumnNames, entityType, sheet.SheetName);
            listAddMethod.Invoke(list, new object[] { entity });
        }
        return list;
    }

    static void ImportExcel(string excelPath, ExcelAssetInfo info)
    {

        string assetName = info.AssetType.Name + ".asset";

        // 直接强制路径，忽略Attribute中的AssetPath设置
        string assetPath = Path.Combine("Assets/Resources/ScriptObject", assetName);

        // 确保目录存在
        Directory.CreateDirectory(Path.GetDirectoryName(assetPath));

        UnityEngine.Object asset = LoadOrCreateAsset(assetPath, info.AssetType);


        IWorkbook book = LoadBook(excelPath);

        var assetFields = info.AssetType.GetFields();
        int sheetCount = 0;

        foreach (var assetField in assetFields)
        {
            ISheet sheet = book.GetSheet(assetField.Name);
            if (sheet == null) continue;

            Type fieldType = assetField.FieldType;
            if (!fieldType.IsGenericType || (fieldType.GetGenericTypeDefinition() != typeof(List<>))) continue;

            Type[] types = fieldType.GetGenericArguments();
            Type entityType = types[0];

            object entities = GetEntityListFromSheet(sheet, entityType);
            assetField.SetValue(asset, entities);
            sheetCount++;
        }


        if (info.Attribute.LogOnImport)
        {
            Debug.Log(string.Format("Imported {0} sheets form {1}.", sheetCount, excelPath));
        }

        EditorUtility.SetDirty(asset);
    }
}

