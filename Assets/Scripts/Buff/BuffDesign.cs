//buff�����buffʱ����ô����
using System;
using UnityEngine;

namespace KidGame.Core
{
    public enum BuffUpdateTimeEnum
    {
        Add,//��ԭ��ʱ��������
        Replace,//�滻Ϊ�µ�ʱ�䣨ˢ�£�
        Keep,//ʱ�䱣�ֲ���
    }

    //buff�Ƴ�ʱ�ĸ��·�ʽ
    public enum BuffRemoveStackUpdateEnum
    {
        Clear,//������Ӵbuff���� 
        Reduce,//����buff����
    }

    public class BuffInfo
    {
        public BuffData buffData;

        public GameObject creator;//������

        public GameObject target;//Ŀ����

        public float durationTimer;//����ʱ���ʱ��

        public float tickTimer;//����ʱ���ʱ��

        public float curStack = 1;//��ǰ����


    }

}
