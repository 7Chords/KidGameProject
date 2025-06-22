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
        //ִ�й�һ�κ���ÿ�
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
            // ��鳡���Ƿ����
            if (!SceneExists(sceneName))
            {
                Debug.LogError($"���� {sceneName} ������");
                return;
            }

            // �첽�����³���
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, _loadSceneMode);
            asyncOperation.allowSceneActivation = false; // �ֶ����Ƴ�������

            // ж�ؾɳ�����������ڣ�
            if (!string.IsNullOrEmpty(_closeSceneName))
            {
                SceneManager.UnloadSceneAsync(_closeSceneName).completed += _ => 
                {
                    _closeSceneName = null;
                };
            }

            // ����Э�̼�ؼ��ؽ���
            StartCoroutine(MonitorLoadingProgress(asyncOperation));
        }

        private IEnumerator MonitorLoadingProgress(AsyncOperation operation)
        {
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f); // 0-1��׼��
                Debug.Log($"���ؽ���: {progress * 100}%");
        
                // ������>=90%ʱ�������
                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }
                yield return null;
            }

            // ������ɻص�
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