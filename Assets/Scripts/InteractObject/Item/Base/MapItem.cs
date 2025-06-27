using KidGame.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    ///  �����еĿ�ʰȡ��Ʒ
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
        /// ��������
        /// </summary>
        public virtual void InteractPositive() { }

        /// <summary>
        /// ��������
        /// </summary>
        public virtual void InteractNegative() { }

        /// <summary>
        /// ʰȡ
        /// </summary>
        public virtual void Pick() { }


    }
}