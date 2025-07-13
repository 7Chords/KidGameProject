using KidGame.Core;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Interface
{
    /// <summary>
    /// 可以互动的接口
    /// </summary>
    public interface IInteractive
    {
        //主动互动
        public abstract void InteractPositive(GameObject interactor);

        //被动互动
        public abstract void InteractNegative(GameObject interactor);
        
        //互动随机音效列表
        public abstract List<string> RandomInteractSfxList { get; set; }

        //互动粒子特效
        public abstract GameObject InteractPartical { get; set; }

    }
}