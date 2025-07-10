using UnityEngine;
using UnityEditor;
using System.IO;

public class CodeLineCounter
{
    [MenuItem("Tools/code lines statistic")]
    static void CountLines()
    {
        string[] guids = AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
        int totalLines = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            int lineCount = File.ReadAllLines(path).Length;
            totalLines += lineCount;
            Debug.Log($"{path} -- {lineCount}. ");
        }

        Debug.Log($"current total number of code lines:{totalLines} .");
    }
}