using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Start���ñ�֤����������ɳ�ʼ��
    private void Start()
    {
        InitGame();
    }

    
    private void InitGame()
    {
        //��Ϸ����ģ��ĳ�ʼ��
        PlayerManager.Instance.Init();
        MapManager.Instance.Init();
    }

    private void DiscardGame()
    {
        //��Ϸ����ģ������� ��Ҫ���¼��ķ�ע��
        PlayerManager.Instance.Discard();
        MapManager.Instance.Discard();
    }
}
