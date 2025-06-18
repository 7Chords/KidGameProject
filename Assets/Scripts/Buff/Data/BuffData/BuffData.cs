using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    [CreateAssetMenu(fileName = "New BuffData", menuName = "KidGameSO/Buff/BuffData")]
    public class BuffData : ScriptableObject
    {
        //基本信息
        public string id;

        public string buffName;

        public string description;

        public Sprite icon;

        public int priority;//优先级 例如中毒和烧伤先执行哪一个

        public int maxStack;//最大可叠层数量

        public string[] tags;//buff的一些特征标记 伤害型等 

        //时间信息

        public bool isForever;//是否是永久buff

        public float duration;//持续时间

        public float tickTime;//间隔触发时间

        //更新方式

        public BuffUpdateTimeEnum buffUpdateTime;

        public BuffRemoveStackUpdateEnum buffRemoveStackUpdate;

        //基础回调点
        public BaseBuffModule OnCreate;

        public BaseBuffModule OnRemove;

        public BaseBuffModule OnTick;

        //伤害回调点

        public BaseBuffModule OnHit;

        public BaseBuffModule OnBeHurt;

        public BaseBuffModule OnKill;

        public BaseBuffModule OnBeKill;

        //etc...


    }
}
