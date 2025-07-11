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
        // todo.根据当前存档的进度重新开始游戏
        // 还没想好在哪里读数据，暂时不写
    }

    public void UI_Quit()
    {
        
    }
}
