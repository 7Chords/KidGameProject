using KidGame.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    ///  �����еĿ�ʰȡ��Ʒ
    /// </summary>
    public class MapItem : MapEntity, IPickable
    {
        /// <summary>
        /// ʰȡ
        /// </summary>
        public virtual void Pick() { }


    }
}