using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public enum TrapState
    {
        NoReady,//未准备完成状态
        Ready,//准备状态 可以被触发
        Running,//触发状态
        Dead,//使用完成状态
    }



}
