using System;
using System.Collections.Generic;
using Utils;
using UnityEngine;

namespace UIFramework.Examples
{
    public class PlayerDataUpdatedSignal : ASignal<List<PlayerDataEntry>>
    {
    }

    [Serializable]
    public class PlayerDataEntry
    {
        public string LevelName;
        [Range(0, 3)] public int Stars;
    }

    [CreateAssetMenu(fileName = "PlayerData", menuName = "UI/Fake Player Data")]
    public class FakePlayerData : ScriptableObject
    {
        [SerializeField] private List<PlayerDataEntry> levelProgress = null;

        public List<PlayerDataEntry> LevelProgress
        {
            get { return levelProgress; }
        }

        /// <summary>
        /// 这个方法是由Unity编辑器在MonoBehaviours和ScriptableObjects中调用的，
        /// 每当检查器中的值发生变化时就会触发。在这里，我使用它来传播示例中的更改，
        /// 但在实践中，你可以通过向屏幕传递一个可观察变量来实现相同的行为，
        /// 该变量通过其属性或其他形式的数据绑定到控制器。
        /// </summary>
        private void OnValidate()
        {
            Signals.Get<PlayerDataUpdatedSignal>().Dispatch(levelProgress);
        }
    }
}

