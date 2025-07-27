//buff�����buffʱ����ô����

using System;
using UnityEngine;

namespace KidGame.Core
{
    public class BuffInfo
    {
        public BuffData buffData;

        public GameObject target;

        public float durationTimer; //����ʱ���ʱ��

        public float tickTimer; //����ʱ���ʱ��

        public float curStack = 1; //��ǰ����

        public object[] exParams; //�Զ��崫�ݵ�һЩ�������

        public BuffInfo(BuffData buffData, GameObject target, object[] paramArr = null)
        {
            this.buffData = buffData;
            this.target = target;
            exParams = paramArr;
        }
    }
}