using UnityEngine;

namespace KidGame.Interface
{
    /// <summary>
    /// ���������ʾ����Ľӿڣ������巶Χ/����...)
    /// </summary>
    public interface IMouseShowDetail
    {
        public abstract GameObject DetailGO { get; set; }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        public abstract void ShowDetail();

        /// <summary>
        /// ��������
        /// </summary>
        public abstract void HideDetail();

    }
}
