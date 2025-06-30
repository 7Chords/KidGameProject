using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Interface
{
    /// <summary>
    /// �ɻ��յĽӿ�
    /// </summary>
    public interface IRecyclable
    {
        //���շ���
        public abstract void Recycle();

        //���������Ч�б�
        public abstract List<string> RandomRecycleSfxList { get; set; }

        //����������Ч
        public abstract GameObject RecyclePartical { get; set; }
    }
}
