using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.UI;
using KidGame.UI.Game;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Utils;
public class GamePauseSignal : ASignal
{
}
public class GamePlayPanelController : PanelController
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Signals.Get<GamePauseSignal>().Dispatch();
        }
    }
}
