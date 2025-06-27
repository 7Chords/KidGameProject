using KidGame.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    ///  场景中的可拾取物品
    /// </summary>
    public class MapItem : MonoBehaviour, IPickable,IInteractive
    {


        [SerializeField]
        protected List<string> randomInteractSfxList;
        [SerializeField]
        protected ParticleSystem interactPartical;

        [SerializeField]
        protected List<string> randomPickSfxList;
        [SerializeField]
        protected ParticleSystem pickPartical;

        public List<string> RandomInteractSfxList { get => randomInteractSfxList; set { randomInteractSfxList = value; } }
        public ParticleSystem InteractPartical { get => interactPartical; set { interactPartical = value; } }
        public List<string> RandomPickSfxList { get => randomPickSfxList; set { randomPickSfxList = value; } }
        public ParticleSystem PickPartical { get => pickPartical; set { pickPartical = value; } }

        /// <summary>
        /// 主动交互
        /// </summary>
        public virtual void InteractPositive() { }

        /// <summary>
        /// 被动交互
        /// </summary>
        public virtual void InteractNegative() { }

        /// <summary>
        /// 拾取
        /// </summary>
        public virtual void Pick() { }


    }
}