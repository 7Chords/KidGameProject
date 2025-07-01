//buff叠层后buff时间怎么计算
using System;
using UnityEngine;

namespace KidGame.Core
{
    public enum BuffUpdateTimeEnum
    {
        Add,//在原有时间上增加
        Replace,//替换为新的时间（刷新）
        Keep,//时间保持不变
    }

    //buff移除时的更新方式
    public enum BuffRemoveStackUpdateEnum
    {
        Clear,//消除所哟buff层数 
        Reduce,//减少buff层数
    }

    public class BuffInfo
    {
        public BuffData buffData;

        public GameObject target;

        public float durationTimer;//持续时间计时器

        public float tickTimer;//作用时间计时器

        public float curStack = 1;//当前层数

        public object[] exParams;//自定义传递的一些额外参数
        public BuffInfo(BuffData buffData, GameObject target, object[] paramArr = null)
        {
            this.buffData = buffData;
            this.target = target;
            exParams = paramArr;
        }
    }

}
