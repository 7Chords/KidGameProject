using KidGame.UI;
using UnityEngine;
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