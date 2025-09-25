using System.Collections.Generic;
using KidGame.Interface;
using UnityEngine;

namespace KidGame.Core
{
    public partial class PlayerController
    {
        /// <summary>
        /// 制造声音
        /// </summary>
        public void ProduceSound(float range)
        {
            List<ISoundable> receiverList = LogicSoundManager.Instance.GetAllReceiver(this, range);
            if (receiverList == null || receiverList.Count == 0) return;
            foreach(var soundable in receiverList)
            {
                soundable.ReceiveSound(gameObject);
            }
        }

        /// <summary>
        /// 接收声音
        /// </summary>
        /// <param name="creator"></param>
        public void ReceiveSound(GameObject creator) { }
    }
}