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
        public abstract void InteractPositive();

        //��������
        public abstract void InteractNegative();
        
        //���������Ч�б�
        public abstract List<string> RandomInteractSfxList { get; set; }

        //����������Ч
        public abstract ParticleSystem InteractPartical { get; set; }

        public abstract string itemName { get; set;}

    }
}