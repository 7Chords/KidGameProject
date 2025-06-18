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
        /// buff��Ч�����ں��������ڼ�ʱ
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
        /// ���buff
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
                    //���ݲ�ͬ��buffʱ�����������������
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
                    //��������buffʱ�Ļص�
                    findBuffInfo.buffData.OnCreate.Apply(findBuffInfo);

                }
            }
            else
            {
                buffInfo.durationTimer = buffInfo.buffData.duration;
                buffInfo.buffData.OnCreate.Apply(buffInfo);
                buffList.Add(buffInfo);
                //��buffList��������
                //�������ȼ��������� ���ȼ�����Խ���ŵ�Խǰ��
                buffList.Sort((buff1, buff2) => buff2.buffData.priority.CompareTo(buff1.buffData.priority));
            }
        }

        /// <summary>
        /// �Ƴ�buff
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
        /// �����б��е�buff
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
            return default;//defalu�����������;���null ����ֵ���;���0֮���
        }

    }
}
