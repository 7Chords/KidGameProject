using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class BuffHandler : MonoBehaviour
    {
        public List<BuffInfo> buffList = new List<BuffInfo>();


        public void Init()
        {
            MonoManager.Instance.AddUpdateListener(BuffTickAndRemove);
        }

        public void Discard()
        {
            MonoManager.Instance.RemoveUpdateListener(BuffTickAndRemove);
        }



        /// <summary>
        /// buff的效果周期和生命周期计时
        /// </summary>
        private void BuffTickAndRemove()
        {
            List<BuffInfo> deleteBuffList = new List<BuffInfo>();
            foreach (var buffInfo in buffList)
            {
                if (buffInfo.buffData.OnTick != null)
                {
                    if (buffInfo.tickTimer < 0)
                    {
                        buffInfo.buffData.OnTick.Apply(buffInfo);
                        buffInfo.tickTimer = buffInfo.buffData.tickTime;
                    }
                    else
                    {
                        buffInfo.tickTimer -= Time.deltaTime;
                    }
                }
                if (buffInfo.durationTimer < 0)
                {
                    deleteBuffList.Add(buffInfo);
                }
                else
                {
                    buffInfo.durationTimer -= Time.deltaTime;
                }
            }
            foreach (var buffInfo in deleteBuffList)
            {
                RemoveBuff(buffInfo);
            }
        }

        /// <summary>
        /// 添加buff
        /// </summary>
        /// <param name="buffInfo"></param>
        public void AddBuff(BuffInfo buffInfo)
        {
            BuffInfo findBuffInfo = FindBuff(buffInfo.buffData.id);

            if (findBuffInfo != null)
            {
                if (findBuffInfo.curStack < findBuffInfo.buffData.maxStack)
                {
                    findBuffInfo.curStack++;
                    //根据不同的buff时间更新类型做出操作
                    switch (findBuffInfo.buffData.buffUpdateTime)
                    {
                        case BuffUpdateTimeEnum.Add:
                            findBuffInfo.durationTimer += findBuffInfo.buffData.duration;
                            break;
                        case BuffUpdateTimeEnum.Replace:
                            findBuffInfo.durationTimer = findBuffInfo.buffData.duration;
                            break;
                        default:
                            break;
                    }
                    //触发创建buff时的回调
                    findBuffInfo.buffData.OnCreate.Apply(findBuffInfo);

                }
            }
            else
            {
                buffInfo.durationTimer = buffInfo.buffData.duration;
                buffInfo.buffData.OnCreate.Apply(buffInfo);
                buffList.Add(buffInfo);
                //对buffList进行排序
                //按照优先级降序排序 优先级数字越大排得越前面
                buffList.Sort((buff1, buff2) => buff2.buffData.priority.CompareTo(buff1.buffData.priority));
            }
        }

        /// <summary>
        /// 移除buff
        /// </summary>
        /// <param name="buffInfo"></param>

        public void RemoveBuff(BuffInfo buffInfo)
        {
            switch (buffInfo.buffData.buffRemoveStackUpdate)
            {
                case BuffRemoveStackUpdateEnum.Clear:
                    buffList.Remove(buffInfo);
                    break;
                case BuffRemoveStackUpdateEnum.Reduce:
                    buffInfo.curStack--;
                    if (buffInfo.curStack == 0)
                    {
                        buffList.Remove(buffInfo);
                    }
                    else
                    {
                        buffInfo.durationTimer = buffInfo.buffData.duration;
                    }
                    break;
            }
            buffInfo.buffData.OnRemove.Apply(buffInfo);
        }

        /// <summary>
        /// 查找列表中的buff
        /// </summary>
        /// <param name="buffDataID"></param>
        /// <returns></returns>
        private BuffInfo FindBuff(string buffDataID)
        {
            foreach (var buffinfo in buffList)
            {
                if (buffinfo.buffData.id == buffDataID)
                {
                    return buffinfo;
                }
            }
            return default;//defalu对于引用类型就是null 对于值类型就是0之类的
        }

    }
}
