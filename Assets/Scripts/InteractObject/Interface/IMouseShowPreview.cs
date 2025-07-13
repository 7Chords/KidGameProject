using UnityEngine;

namespace KidGame.Interface
{
    /// <summary>
    /// ���������ʾԤ���Ľӿڣ������巶Χ/����...)
    /// </summary>
    public interface IMouseShowPreview
    {
        public abstract GameObject PreviewGO { get; set; }

        /// <summary>
        /// ��ʾԤ������
        /// </summary>
        public abstract void ShowPreview();

        /// <summary>
        /// ����Ԥ������
        /// </summary>
        public abstract void HidePreview();

    }
}
