using KidGame.Core;
using System;
using UnityEditor;
using UnityEngine;
using KidGame;
using NUnit.Framework;
using System.Collections.Generic;

/// <summary>
/// 开发者作弊面板
/// </summary>
public class DeveloperCheatEditor : EditorWindow
{
    //ctrl + shift + k
    [MenuItem("自定义编辑器/开发者作弊面板 %#k")]
    public static void ShowWindow()
    {
        GetWindow<DeveloperCheatEditor>("开发者作弊面板");
    }


    // 折叠状态变量
    private bool showItemCheats = false;
    private bool showTrapCheats = false;
    private bool showMaterialCheats = false;
    private bool showWeaponCheats = true;
    private bool showFoodCheats = true;

    private bool showLevelCheats = false;
    private bool showSysCheats = false;


    #region 道具相关

    private string getTrapIDStr;
    private string getTrapAmountStr;
    private string deleteTrapIDStr;
    private string deleteTrapAmountStr;
    private string getAllTrapAmountStr;

    private string getMatIDStr;
    private string getMatAmountStr;
    private string deleteMatIDStr;
    private string deleteMatAmountStr;
    private string getAllMatAmountStr;

    private string getWeaponIDStr;
    private string getWeaponAmountStr;
    private string deleteWeaponIDStr;
    private string deleteWeaponAmountStr;
    private string getAllWeaponAmountStr;
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
                    GainItem(getTrapIDStr, getTrapAmountStr, UseItemType.trap);
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
                    DeleteItem(deleteTrapIDStr, deleteTrapAmountStr);
                }

                deleteTrapIDStr = EditorGUILayout.TextField("陷阱ID", deleteTrapIDStr);
                deleteTrapAmountStr = EditorGUILayout.TextField("删除陷阱数量", deleteTrapAmountStr);
                EditorGUILayout.EndHorizontal();

                #endregion

                #region 获得所有陷阱
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("获得所有陷阱"))
                {
                    GetAllItem(SoLoader.Instance.GetDataList<TrapData>(), UseItemType.trap, getAllTrapAmountStr);
                }
                getAllTrapAmountStr = EditorGUILayout.TextField("获得陷阱数量", getAllTrapAmountStr);
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


                #region 获得材料

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("获得材料"))
                {
                    GainItem(getMatIDStr, getMatAmountStr, UseItemType.Material);
                }

                // 简单文本输入框
                getMatIDStr = EditorGUILayout.TextField("材料ID", getMatIDStr);
                getMatAmountStr = EditorGUILayout.TextField("获得材料数量", getMatAmountStr);
                EditorGUILayout.EndHorizontal();

                #endregion

                #region 删除材料

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("删除材料"))
                {
                    DeleteItem(deleteMatIDStr, deleteMatAmountStr);
                }

                deleteMatIDStr = EditorGUILayout.TextField("材料ID", deleteMatIDStr);
                deleteMatAmountStr = EditorGUILayout.TextField("删除材料数量", deleteMatAmountStr);
                EditorGUILayout.EndHorizontal();

                #endregion

                #region 获得所有材料
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("获得所有材料"))
                {
                    GetAllItem(SoLoader.Instance.GetDataList<MaterialData>(), UseItemType.Material, getAllMatAmountStr);
                }
                getAllMatAmountStr = EditorGUILayout.TextField("获得材料数量", getAllMatAmountStr);
                EditorGUILayout.EndHorizontal();

                #endregion


                EditorGUI.indentLevel--;
            }

            // 武器作弊 (子分类)
            showWeaponCheats = EditorGUILayout.Foldout(showWeaponCheats, "武器作弊", true);
            if (showWeaponCheats)
            {
                EditorGUI.indentLevel++;


                #region 获得武器

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("获得武器"))
                {
                    GainItem(getWeaponIDStr, getWeaponAmountStr, UseItemType.weapon);
                }

                // 简单文本输入框
                getWeaponIDStr = EditorGUILayout.TextField("武器ID", getWeaponIDStr);
                getWeaponAmountStr = EditorGUILayout.TextField("获得武器数量", getWeaponAmountStr);
                EditorGUILayout.EndHorizontal();

                #endregion

                #region 删除武器

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("删除武器"))
                {
                    DeleteItem(deleteWeaponIDStr, deleteWeaponAmountStr);
                }

                deleteWeaponIDStr = EditorGUILayout.TextField("武器ID", deleteWeaponIDStr);
                deleteWeaponAmountStr = EditorGUILayout.TextField("删除武器数量", deleteWeaponAmountStr);
                EditorGUILayout.EndHorizontal();

                #endregion

                #region 获得所有武器
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("获得所有武器"))
                {
                    GetAllItem(SoLoader.Instance.GetDataList<WeaponData>(), UseItemType.weapon, getAllWeaponAmountStr);
                }
                getAllWeaponAmountStr = EditorGUILayout.TextField("获得武器数量", getAllWeaponAmountStr);
                EditorGUILayout.EndHorizontal();

                #endregion

                EditorGUI.indentLevel--;
            }

            // 食物作弊 (子分类)
            showFoodCheats = EditorGUILayout.Foldout(showFoodCheats, "食物作弊", true);
            if (showFoodCheats)
            {
                EditorGUI.indentLevel++;


                EditorGUI.indentLevel--;
            }


            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }


        // 关卡内相关作弊
        showLevelCheats = EditorGUILayout.Foldout(showLevelCheats, "关卡相关作弊", true);

        showSysCheats = EditorGUILayout.Foldout(showSysCheats, "系统相关作弊", true);
        if(showSysCheats)
        {
            if(GUILayout.Button("输入映射切换（GAME/UI）"))
            {
                MsgCenter.SendMsgAct(MsgConst.ON_CONTROL_MAP_CHG);
            }
        }
        EditorGUILayout.HelpBox("这些作弊功能只在游戏运行时有效", MessageType.Info);

    }

    // 获得所有物品
    private void GetAllItem<T>(List<T> dataList,UseItemType itemType,string amountStr) where T : BagItemInfoBase
    {
        if (string.IsNullOrEmpty(amountStr))
        {
            Debug.LogWarning($"{itemType.ToString()}数量为空");
            return;
        }

        try
        {
            foreach (var data in dataList)
            {
                PlayerBag.Instance.AddItemToCombineBag(data.Id, itemType,
                    int.Parse(amountStr));
            }

        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    // 获得物品
    private void GainItem(string idStr, string amountStr, UseItemType itemType)
    {
        if (string.IsNullOrEmpty(idStr) || string.IsNullOrEmpty(amountStr)) return;
        try
        {
            PlayerBag.Instance.AddItemToCombineBag(idStr, itemType,
                int.Parse(amountStr));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    // 删除物品
    private void DeleteItem(string _deleteIDStr, string _deleteAmountStr)
    {
        if (string.IsNullOrEmpty(_deleteIDStr) || string.IsNullOrEmpty(_deleteAmountStr)) return;
        try
        {
            PlayerBag.Instance.DeleteItemInCombineBag(_deleteIDStr, int.Parse(_deleteAmountStr));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

}