using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ��Դ��ȡ������ Э����ȡ�������д·������Դ
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// ����·����Resources�£���ȡSprite
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Sprite GetSpriteByPath(string path)
        {
            return Resources.Load<Sprite>(path);
        }

        /// <summary>
        /// ����·����Resources�£���ȡ
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static BaseBuffModule GetBuffModule(string path)
        {
            return Resources.Load<BaseBuffModule>(path);
        }
    }
}
