using KidGame.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    ///  场景中的可拾取物品
    /// </summary>
    public class MapItem : MapEntity, IPickable
    {

        [SerializeField]
        protected List<string> randomPickSfxList;
        [SerializeField]
        protected GameObject pickPartical;

        public List<string> RandomPickSfxList { get => randomPickSfxList; set { randomPickSfxList = value; } }
        public GameObject PickPartical { get => pickPartical; set { pickPartical = value; } }

        /// <summary>
        /// 拾取
        /// </summary>
        public virtual void Pick() { }


    }
}