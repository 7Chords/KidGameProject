using UnityEngine;

namespace KidGame.Interface
{
    /// <summary>
    /// 鼠标移入显示详情的接口（如陷阱范围/敌人...)
    /// </summary>
    public interface IMouseShowDetail
    {
        public abstract GameObject DetailGO { get; set; }

        /// <summary>
        /// 显示详情
        /// </summary>
        public abstract void ShowDetail();

        /// <summary>
        /// 隐藏详情
        /// </summary>
        public abstract void HideDetail();

    }
}
