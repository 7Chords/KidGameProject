using KidGame.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 场景中的装饰物，不需要实现具体的逻辑
    /// </summary>
    public class MapDecoration : MonoBehaviour, IInteractive
    {
        [SerializeField]
        protected List<string> randomInteractSfxList;
        public List<string> RandomInteractSfxList { get => randomInteractSfxList; set { randomInteractSfxList = value; } }

        [SerializeField]
        protected GameObject interactPartical;
        public GameObject InteractPartical { get => interactPartical; set { interactPartical = value; } }

        public string itemName { get => "好看的装饰物"; set { } }

        public virtual void InteractPositive(GameObject interactor) { }

        public virtual void InteractNegative(CatalystBase catalyst, GameObject interactor) { }//不会去写任何东西
    }
}