using KidGame.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    ///  场景中的可拾取物品
    /// </summary>
    public class MapItem : MapEntity, IPickable
    {
        /// <summary>
        /// 拾取
        /// </summary>
        public virtual void Pick() { }


    }
}