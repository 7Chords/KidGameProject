using KidGame.Core;
using KidGame.UI;
using KidGame.UI.Game;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class OpenPauseSignal : ASignal
{
}

public class PauseWindowController : WindowController
{
    [Header("UI Elements")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private bool isPaused = false;

    private void Start()
    {
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(UI_Setting);
        quitButton.onClick.AddListener(UI_Quit);
    }

    /// <summary>
    /// �������ʱ���������������
    /// </summary>
    protected override void OnPropertiesSet()
    {
        TogglePause();
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        GameManager.Instance.GamePause();
        Show();
    }

    private void ResumeGame()
    {
        GameManager.Instance.GameResume();
        Hide();
        isPaused = false;
    }

    #region ��ť����

    public void UI_Setting()
    {
        Signals.Get<ToSettingsWindowSignal>().Dispatch();
    }
    
    public void UI_Quit()
    {
        SceneLoader.Instance.LoadSceneWithTransition("StartScene",
            UIController.Instance.UICameraBindingVertexCamera);
    }    

    #endregion
    
    protected override void OnDestroy()
    {
        resumeButton.onClick.RemoveListener(ResumeGame);
        
        base.OnDestroy();
    }
}