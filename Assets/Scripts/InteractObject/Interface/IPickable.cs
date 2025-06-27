using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Interface
{
    /// <summary>
    /// ��ʰȡ�ӿ�
    /// </summary>
    public interface IPickable
    {
        public abstract void Pick();

        //���������Ч�б�
        public abstract List<string> RandomPickSfxList { get; set; }

        //����������Ч
        public abstract ParticleSystem PickPartical { get; set; }
    }
}