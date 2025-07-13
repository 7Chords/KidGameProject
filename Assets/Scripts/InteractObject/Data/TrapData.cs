using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    //���崥��ö��
    public enum TrapTriggerType
    {
        Positive, //������
        Negative, //������
    }

    //������Чö��
    public enum TrapValidType
    {
        Immediate,//������Ч
        TimeDelay, //�ȴ�һ��ʱ�����Ч
    }

    //�������ö��
    public enum TrapPlacedType
    {
        Ground,
        Furniture,
    }

    //��������ö��
    public enum TrapDeadType
    {
        Immediate,      // ��������
        TimeDelay,      // �ӳ�����
        ExternalEvent   // �ⲿ�¼���������
    }

    [System.Serializable]
    public class TrapDeadConfig
    {
        public List<TrapDeadType> conditions;

        [Tooltip("��������TimeDelayʱ��Ч")]
        public float delayTime;
    }


    [CreateAssetMenu(fileName = "TrapData", menuName = "KidGameSO/Interactive/TrapData")]
    public class TrapData : ScriptableObject
    {
        public string trapID;
        public string trapName;
        public string trapDesc;
        public Sprite trapIcon;
        public float trapLevel;
        public float trapDamage; //����������˺��ǻ����˺� ���������˺�������������Ӱ�� ��trap��mono�ű��д������
        public TrapTriggerType triggerType;
        public TrapValidType validType;
        public float validTime = -1; //��Чʱ�� -1Ϊ����������Ч
        public TrapPlacedType placedType;
        public List<TrapDeadType> deadTypeList;
        public float deadDelayTime;//�����ӳ�ʱ�� deadTypeList����TimeDelay����Ч
        //public bool isCatalyst;//�Ƿ��Ǵ�ý
    }
}