using KidGame.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    ///  �����еĿ�ʰȡ��Ʒ
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
        /// ʰȡ
        /// </summary>
        public virtual void Pick() { }


    }
}