using KidGame.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// �����е�װ�������Ҫʵ�־�����߼�
    /// </summary>
    public class MapDecoration : MonoBehaviour, IInteractive
    {
        [SerializeField]
        protected List<string> randomInteractSfxList;
        public List<string> RandomInteractSfxList { get => randomInteractSfxList; set { randomInteractSfxList = value; } }

        [SerializeField]
        protected GameObject interactPartical;
        public GameObject InteractPartical { get => interactPartical; set { interactPartical = value; } }

        public string itemName { get => "�ÿ���װ����"; set { } }

        public virtual void InteractNegative(GameObject interactor) { }

        public virtual void InteractPositive(GameObject interactor) { }
    }
}