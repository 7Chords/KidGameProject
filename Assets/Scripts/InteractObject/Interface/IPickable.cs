using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Interface
{
    /// <summary>
    /// 可拾取接口
    /// </summary>
    public interface IPickable
    {
        public abstract void Pick();

        //捡起随机音效列表
        public abstract List<string> RandomPickSfxList { get; set; }

        //捡起粒子特效
        public abstract GameObject PickPartical { get; set; }

        public abstract string itemName { get; set; }
    }
}