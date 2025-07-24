using KidGame.Core;
using System;
using UnityEditor;
using UnityEngine;
using KidGame;

/// <summary>
/// 开发者作弊面板
/// </summary>
public class DeveloperCheatEditor : EditorWindow
{
    [MenuItem("自定义编辑器/开发者作弊面板")]
    public static void ShowWindow()
    {
        GetWindow<DeveloperCheatEditor>("开发者作弊面板");
    }


    // 折叠状态变量
    private bool showItemCheats = false;
    private bool showTrapCheats = false;
    private bool showMaterialCheats = false;
    private bool showPlayerCheats = true;
    private bool showLevelCheats = true;


    #region 道具相关

    private string getTrapIDStr;
    private string getTrapAmountStr;

    private string deleteTrapIDStr;
    private string deleteTrapAmountStr;

    #endregion

    private void OnGUI()
    {
        GUILayout.Label("################开发者作弊面板################", EditorStyles.boldLabel);

        EditorGUILayout.Space();


        // 道具相关作弊
        showItemCheats = EditorGUILayout.Foldout(showItemCheats, "道具相关作弊", true);
        if (showItemCheats)
        {
            EditorGUI.indentLevel++;

            // 陷阱作弊 (子分类)
            showTrapCheats = EditorGUILayout.Foldout(showTrapCheats, "陷阱作弊", true);
            if (showTrapCheats)
            {
                EditorGUI.indentLevel++;

                #region 获得陷阱

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("获得陷阱"))
                {
                    if (string.IsNullOrEmpty(getTrapIDStr) || string.IsNullOrEmpty(getTrapAmountStr)) return;
                    try
                    {
                        PlayerBag.Instance.AddItemToCombineBag(getTrapIDStr, UseItemType.trap,
                            int.Parse(getTrapAmountStr));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
                }

                // 简单文本输入框
                getTrapIDStr = EditorGUILayout.TextField("陷阱ID", getTrapIDStr);
                getTrapAmountStr = EditorGUILayout.TextField("获得陷阱数量", getTrapAmountStr);
                EditorGUILayout.EndHorizontal();

                #endregion


                #region 删除陷阱

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("删除陷阱"))
                {
                    if (string.IsNullOrEmpty(deleteTrapIDStr) || string.IsNullOrEmpty(deleteTrapAmountStr)) return;
                    try
                    {
                        PlayerBag.Instance.DeleteItemInCombineBag(deleteTrapIDStr, int.Parse(deleteTrapAmountStr));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
                }

                deleteTrapIDStr = EditorGUILayout.TextField("陷阱ID", deleteTrapIDStr);
                deleteTrapAmountStr = EditorGUILayout.TextField("删除陷阱数量", deleteTrapAmountStr);
                EditorGUILayout.EndHorizontal();

                #endregion


                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            // 材料作弊 (子分类)
            showMaterialCheats = EditorGUILayout.Foldout(showMaterialCheats, "材料作弊", true);
            if (showMaterialCheats)
            {
                EditorGUI.indentLevel++;

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        //// 玩家相关作弊
        //showPlayerCheats = EditorGUILayout.Foldout(showPlayerCheats, "玩家作弊", true);
        //if (showPlayerCheats)
        //{
        //    EditorGUI.indentLevel++;

        //    if (GUILayout.Button("Give 1000 Gold"))
        //    {

        //    }

        //    if (GUILayout.Button("God Mode"))
        //    {

        //    }

        //    if (GUILayout.Button("Reset Player"))
        //    {

        //    }

        //    EditorGUI.indentLevel--;
        //    EditorGUILayout.Space();
        //}

        //// 关卡相关作弊
        //showLevelCheats = EditorGUILayout.Foldout(showLevelCheats, "关卡作弊", true);
        //if (showLevelCheats)
        //{
        //    EditorGUI.indentLevel++;

        //    if (GUILayout.Button("Unlock All Levels"))
        //    {
        //        ExecuteInPlayMode(UnlockAllLevels);
        //    }

        //    if (GUILayout.Button("Skip Level"))
        //    {
        //        ExecuteInPlayMode(SkipCurrentLevel);
        //    }

        //    EditorGUI.indentLevel--;
        //    EditorGUILayout.Space();
        //}


        EditorGUILayout.HelpBox("这些作弊功能只在游戏运行时有效", MessageType.Info);
    }

    /// <summary>
    /// 辅助方法：确保只在游戏运行时执行
    /// </summary>
    /// <param name="action"></param>
    private void ExecuteInPlayMode(System.Action action)
    {
        if (Application.isPlaying)
        {
            action?.Invoke();
        }
        else
        {
            Debug.LogWarning("作弊功能只在游戏运行时有效！");
        }
    }

    #region Callback

    #endregion
}