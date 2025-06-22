using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KidGame.UI.Game;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace KidGame.Core
{
    [System.Serializable]
    public class TransitionPreset
    {
        public string presetName;
        public string screenId;
    }

    public class SceneLoader : SingletonPersistent<SceneLoader>
    {
        [SerializeField] private List<TransitionPreset> transitionPresets;
        [SerializeField] private string defaultPresetName = "BlackFade";

        private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;
        //执行过一次后会置空
        private string _closeSceneName;
        private Action _onSceneLoaded;
        
        private Dictionary<string, string> presetDict;

        protected override void Awake()
        {
            base.Awake();

            presetDict = new Dictionary<string, string>();
            foreach (var preset in transitionPresets)
            {
                if (!presetDict.ContainsKey(preset.presetName))
                {
                    presetDict.Add(preset.presetName, preset.screenId);
                }
            }
        }

        public void LoadSceneWithTransition(string sceneName,Action onFinish = null, string presetName = null,LoadSceneMode loadSceneMode = LoadSceneMode.Single,string closeSceneName = null)
        {
            _loadSceneMode = loadSceneMode;
            _closeSceneName = closeSceneName;
            _onSceneLoaded = onFinish;
            if (!presetDict.TryGetValue(presetName ?? defaultPresetName, out var screenId))
                return;

            var panel = UIController.Instance.GetTransitionPanel();

            if (panel == null)
            {
                Debug.LogError("TransitionPanel not found for " + presetName);
                return;
            }

            panel.BeginTransitionAndLoadScene(sceneName);
        }

        public void LoadScene(string sceneName, Action callback = null)
        {
            // 检查场景是否存在
            if (!SceneExists(sceneName))
            {
                Debug.LogError($"场景 {sceneName} 不存在");
                return;
            }

            // 异步加载新场景
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, _loadSceneMode);
            asyncOperation.allowSceneActivation = false; // 手动控制场景激活

            // 卸载旧场景（如果存在）
            if (!string.IsNullOrEmpty(_closeSceneName))
            {
                SceneManager.UnloadSceneAsync(_closeSceneName).completed += _ => 
                {
                    _closeSceneName = null;
                };
            }

            // 启动协程监控加载进度
            StartCoroutine(MonitorLoadingProgress(asyncOperation));
        }

        private IEnumerator MonitorLoadingProgress(AsyncOperation operation)
        {
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f); // 0-1标准化
                Debug.Log($"加载进度: {progress * 100}%");
        
                // 当进度>=90%时允许激活场景
                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }
                yield return null;
            }

            // 加载完成回调
            _onSceneLoaded?.Invoke();
            _onSceneLoaded = null;
        }

        private bool SceneExists(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (System.IO.Path.GetFileNameWithoutExtension(scenePath) == sceneName)
                    return true;
            }
            return false;
        }
    }
}