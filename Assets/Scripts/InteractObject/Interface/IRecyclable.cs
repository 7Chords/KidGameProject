using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Interface
{
    /// <summary>
    /// 可回收的接口
    /// </summary>
    public interface IRecyclable
    {
        //回收方法
        public abstract void Recycle();

        //回收随机音效列表
        public abstract List<string> RandomRecycleSfxList { get; set; }

        //回收粒子特效
        public abstract GameObject RecyclePartical { get; set; }
    }
}
