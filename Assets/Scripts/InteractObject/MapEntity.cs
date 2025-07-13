using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// 地图场景物体基类
    /// </summary>
    public class MapEntity : MonoBehaviour
    {
        public virtual string EntityName { get => "default entity name"; set { } }
    }
}
