using KidGame.Core;
using System;
using UnityEditor;
using UnityEngine;
using KidGame;

/// <summary>
/// �������������
/// </summary>
public class DeveloperCheatEditor : EditorWindow
{
    [MenuItem("�Զ���༭��/�������������")]
    public static void ShowWindow()
    {
        GetWindow<DeveloperCheatEditor>("�������������");
    }


    // �۵�״̬����
    private bool showItemCheats = false;
    private bool showTrapCheats = false;
    private bool showMaterialCheats = false;
    private bool showPlayerCheats = true;
    private bool showLevelCheats = true;


    #region �������
    private string trapIDStr;
    private string trapAmountStr;

    #endregion
    private void OnGUI()
    {
        GUILayout.Label("�������������(KidGameCheatPanel) :-)", EditorStyles.boldLabel);

        EditorGUILayout.Space();


        // �����������
        showItemCheats = EditorGUILayout.Foldout(showItemCheats, "�����������", true);
        if (showItemCheats)
        {
            EditorGUI.indentLevel++;

            // �������� (�ӷ���)
            showTrapCheats = EditorGUILayout.Foldout(showTrapCheats, "��������", true);
            if (showTrapCheats)
            {
                EditorGUI.indentLevel++;
                // ����ָ��ˮƽ����
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("�������"))
                {
                    if (string.IsNullOrEmpty(trapIDStr) || string.IsNullOrEmpty(trapAmountStr)) return;
                    try
                    {
                        PlayerBag.Instance.AddItemToCombineBag(trapIDStr, UseItemType.trap, int.Parse(trapAmountStr));
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
                }

                // ���ı������
                trapIDStr = EditorGUILayout.TextField("����ID", trapIDStr);
                trapAmountStr = EditorGUILayout.TextField("��������", trapAmountStr);
                EditorGUILayout.EndHorizontal();





                EditorGUI.indentLevel--;
                EditorGUILayout.Space();

            }

            // �������� (�ӷ���)
            showMaterialCheats = EditorGUILayout.Foldout(showMaterialCheats, "��������", true);
            if (showMaterialCheats)
            {
                EditorGUI.indentLevel++;

                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        //// ����������
        //showPlayerCheats = EditorGUILayout.Foldout(showPlayerCheats, "�������", true);
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

        //// �ؿ��������
        //showLevelCheats = EditorGUILayout.Foldout(showLevelCheats, "�ؿ�����", true);
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


        EditorGUILayout.HelpBox("��Щ���׹���ֻ����Ϸ����ʱ��Ч", MessageType.Info);
    }
    /// <summary>
    /// ����������ȷ��ֻ����Ϸ����ʱִ��
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
            Debug.LogWarning("���׹���ֻ����Ϸ����ʱ��Ч��");
        }
    }

    #region Callback


    #endregion
}