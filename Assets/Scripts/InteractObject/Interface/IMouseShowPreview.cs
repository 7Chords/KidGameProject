using UnityEngine;

namespace KidGame.Interface
{
    /// <summary>
    /// 鼠标移入显示预览的接口（如陷阱范围/敌人...)
    /// </summary>
    public interface IMouseShowPreview
    {
        public abstract GameObject PreviewGO { get; set; }

        /// <summary>
        /// 显示预览内容
        /// </summary>
        public abstract void ShowPreview();

        /// <summary>
        /// 隐藏预览内容
        /// </summary>
        public abstract void HidePreview();

    }
}
