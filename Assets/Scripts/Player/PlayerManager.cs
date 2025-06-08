using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ҹ����� ������Ҹ���ģ��ĳ�ʼ������ ����˳��
/// ��ͬ��ҲҪ��GameManager��ʼ��
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    public void Init()
    {
        PlayerUtil.Instance.Init();
        PlayerController.Instance.Init();
        PlayerBag.Instance.Init();
    }

    public void Discard()
    {
        PlayerUtil.Instance.Discard();
        PlayerController.Instance.Discard();
        PlayerBag.Instance.Discard();
    }
}