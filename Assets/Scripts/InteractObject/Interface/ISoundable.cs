using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Interface
{
    /// <summary>
    /// 具有声音逻辑功能的接口
    /// </summary>
    public interface ISoundable
    {
        //发出声音(逻辑层面的）
        public abstract void ProduceSound(float range);

        //接收声音(逻辑层面的）
        public abstract void ReceiveSound(GameObject creator);
    }

}
