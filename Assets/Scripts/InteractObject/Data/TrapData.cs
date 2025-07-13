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

    //��������ö��
    public enum TrapDeadType
    {
        Immediate,      // ��������
        TimeDelay,      // �ӳ�����
        ExternalEvent   // �ⲿ�¼���������
    }


    [CreateAssetMenu(fileName = "TrapData", menuName = "KidGameSO/Interactive/TrapData")]
    public class TrapData : ScriptableObject
    {
        public string trapID;
        public string trapName;
        public string trapDesc;
        public string trapIconPath;
        public float trapLevel;
        public float trapDamage; //����������˺��ǻ����˺� ���������˺�������������Ӱ�� ��trap��mono�ű��д������
        public TrapTriggerType triggerType;
        public TrapValidType validType;
        public float validTime = -1; //��Чʱ�� -1Ϊ����������Ч
        public List<TrapDeadType> deadTypeList;
        public float deadDelayTime;//�����ӳ�ʱ�� deadTypeList����TimeDelay����Ч
        public string interactSoundPath;
        public string pickSoundPath;
        public string workSoundPath;
        public string deadSoundPath;
        public string interactParticalPath;
        public string pickParticalPath;
        public string workParticalPath;
        public string deadParticalPath;

    }
}