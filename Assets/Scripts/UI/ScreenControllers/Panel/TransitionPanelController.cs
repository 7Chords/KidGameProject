using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KidGame.UI;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using KidGame.UI.Game;

public class TransitionPanelController : PanelController
{
    [SerializeField] private Image overlayImage;
    [SerializeField] private float fadeDuration = 0.5f;

    private string sceneToLoad;

    public void FadeIn(Action onComplete = null)
    {
        overlayImage.DOFade(1f, fadeDuration).OnComplete(() => onComplete?.Invoke());
    }

    public void FadeOut(Action onComplete = null)
    {
        overlayImage.DOFade(0f, fadeDuration).OnComplete(() => onComplete?.Invoke());
    }

    /// <summary>
    /// 执行完整的场景转场逻辑
    /// </summary>
    public void BeginTransitionAndLoadScene(string sceneName)
    {
        sceneToLoad = sceneName;
        StartCoroutine(DoTransition());
    }

    private IEnumerator DoTransition()
    {
        bool isInDone = false;
        FadeIn(() => isInDone = true);

        yield return new WaitUntil(() => isInDone);

        SceneManager.LoadScene(sceneToLoad);

        yield return null;

        bool isOutDone = false;
        FadeOut(() =>
        {
            isOutDone = true;
            UIController.Instance.HideTransitionPanel();
        });

        yield return new WaitUntil(() => isOutDone);
    }

    protected override void OnPropertiesSet()
    {
        overlayImage.color = new Color(overlayImage.color.r, overlayImage.color.g, overlayImage.color.b, 0);
    }
}
