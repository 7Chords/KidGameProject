using KidGame.Core;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Interface
{
    /// <summary>
    /// ���Ի����Ľӿ�
    /// </summary>
    public interface IInteractive
    {
        //��������
        public abstract void InteractPositive(GameObject interactor);

        //��������
        public abstract void InteractNegative(GameObject interactor);
        
        //���������Ч�б�
        public abstract List<string> RandomInteractSfxList { get; set; }

        //����������Ч
        public abstract GameObject InteractPartical { get; set; }

    }
}