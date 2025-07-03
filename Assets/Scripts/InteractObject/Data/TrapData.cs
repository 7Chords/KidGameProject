using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
//����ö�� �����
    public enum TrapType
    {
        Positive, //������
        Negative, //������
        Time_Valid, //�ȴ�һ��ʱ�����Ч
    }

//���崥��ö��
    public enum TrapTriggerType
    {
        None,
        Self, //�Լ�������
        Catalyst, //��ý����
    }
//�������ö��
    public enum TrapPlacedType
    {
        Normal,
        Attach2Furniture,
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
        public float validTime = -1; //��Чʱ�� -1Ϊ����������Ч
        public List<TrapType> trapTypeList;
        public TrapTriggerType triggerType;
        public TrapPlacedType placedType;
        public TrapDeadConfig deadConfig;
        public bool isCatalyst;//�Ƿ��Ǵ�ý
    }
}