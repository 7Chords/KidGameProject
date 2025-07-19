using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 资源获取帮助类 协助获取配表中填写路径的资源
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// 根据路径（Resources下）获取Sprite
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Sprite GetSpriteByPath(string path)
        {
            return Resources.Load<Sprite>(path);
        }

        /// <summary>
        /// 根据路径（Resources下）获取
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static BaseBuffModule GetBuffModule(string path)
        {
            return Resources.Load<BaseBuffModule>(path);
        }
    }
}
