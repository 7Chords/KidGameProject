using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using KidGame.UI;
using UnityEngine;
using UnityEngine.UI;

public class GameFailWindowController : WindowController
{
    public Button restartButton;
    public Button quitButton;

    private void Start()
    {
        restartButton.onClick.AddListener(UI_Restart);
        quitButton.onClick.AddListener(UI_Quit);
    }

    public void UI_Restart()
    {
        // todo.���ݵ�ǰ�浵�Ľ������¿�ʼ��Ϸ
        // ��û�������������ݣ���ʱ��д
    }

    public void UI_Quit()
    {
        
    }
}
