using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Interface
{
    /// <summary>
    /// ���������߼����ܵĽӿ�
    /// </summary>
    public interface ISoundable
    {
        //��������(�߼�����ģ�
        public abstract void ProduceSound(float range);

        //��������(�߼�����ģ�
        public abstract void ReceiveSound(GameObject creator);
    }

}
