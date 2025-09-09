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

        /// <summary>
        /// 接收声音的范围
        /// </summary>
        public abstract float ReceiveSoundRange { get; }

        /// <summary>
        /// 获得该物体
        /// </summary>
        public abstract GameObject SoundGameObject { get; }
    }

}
