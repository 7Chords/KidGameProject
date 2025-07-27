//buff叠层后buff时间怎么计算

using System;
using UnityEngine;

namespace KidGame.Core
{
    public class BuffInfo
    {
        public BuffData buffData;

        public GameObject target;

        public float durationTimer; //持续时间计时器

        public float tickTimer; //作用时间计时器

        public float curStack = 1; //当前层数

        public object[] exParams; //自定义传递的一些额外参数

        public BuffInfo(BuffData buffData, GameObject target, object[] paramArr = null)
        {
            this.buffData = buffData;
            this.target = target;
            exParams = paramArr;
        }
    }
}