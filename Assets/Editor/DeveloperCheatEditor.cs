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

                #region 获得所有陷阱
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("获得所有陷阱"))
                {
                    if (string.IsNullOrEmpty(getAllTrapAmountStr)) return;
                    try
                    {
                        foreach (var data in SoLoader.Instance.GetGameDataConfig().TrapDataList)
                        {
                            PlayerBag.Instance.AddItemToCombineBag(data.Id, UseItemType.trap,
                                int.Parse(getAllTrapAmountStr));
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
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
                    if (string.IsNullOrEmpty(getMatIDStr) || string.IsNullOrEmpty(getMatAmountStr)) return;
                    try
                    {
                        PlayerBag.Instance.AddItemToCombineBag(getMatIDStr, UseItemType.Material,
                            int.Parse(getMatAmountStr));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
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
                    if (string.IsNullOrEmpty(deleteMatIDStr) || string.IsNullOrEmpty(deleteMatAmountStr)) return;
                    try
                    {
                        PlayerBag.Instance.DeleteItemInCombineBag(deleteMatIDStr, int.Parse(deleteMatAmountStr));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
                }

                deleteMatIDStr = EditorGUILayout.TextField("材料ID", deleteMatIDStr);
                deleteMatAmountStr = EditorGUILayout.TextField("删除材料数量", deleteMatAmountStr);
                EditorGUILayout.EndHorizontal();

                #endregion

                #region 获得所有材料
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("获得所有材料"))
                {
                    if (string.IsNullOrEmpty(getAllMatAmountStr)) return;
                    try
                    {
                        foreach (var data in SoLoader.Instance.GetGameDataConfig().MaterialDataList)
                        {
                            PlayerBag.Instance.AddItemToCombineBag(data.Id, UseItemType.Material,
                                int.Parse(getAllMatAmountStr));
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
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
                    if (string.IsNullOrEmpty(getWeaponIDStr) || string.IsNullOrEmpty(getWeaponAmountStr)) return;
                    try
                    {
                        PlayerBag.Instance.AddItemToCombineBag(getWeaponIDStr, UseItemType.weapon,
                            int.Parse(getWeaponAmountStr));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }

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
                    if (string.IsNullOrEmpty(deleteWeaponIDStr) || string.IsNullOrEmpty(deleteWeaponAmountStr)) return;
                    try
                    {
                        PlayerBag.Instance.DeleteItemInCombineBag(deleteWeaponIDStr, int.Parse(deleteWeaponAmountStr));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
                }

                deleteWeaponIDStr = EditorGUILayout.TextField("武器ID", deleteWeaponIDStr);
                deleteWeaponAmountStr = EditorGUILayout.TextField("删除武器数量", deleteWeaponAmountStr);
                EditorGUILayout.EndHorizontal();

                #endregion

                #region 获得所有武器
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("获得所有武器"))
                {
                    if (string.IsNullOrEmpty(getAllWeaponAmountStr)) return;
                    try
                    {
                        foreach (var data in SoLoader.Instance.GetGameDataConfig().WeaponDataList)
                        {
                            PlayerBag.Instance.AddItemToCombineBag(data.Id, UseItemType.weapon,
                                int.Parse(getAllWeaponAmountStr));
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
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

        EditorGUILayout.HelpBox("这些作弊功能只在游戏运行时有效", MessageType.Info);

    }

}