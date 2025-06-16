using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KidGame.UI.Game;

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

        public void LoadScene(string sceneName, string presetName = null)
        {
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
    }
}