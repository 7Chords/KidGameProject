using KidGame;
using KidGame.Core;
using System;
using System.Collections.Generic;

[Serializable]
public class TrapData : BagItemInfoBase
{
    public string id;
    public string trapName;
    public string trapDesc;
    public string trapIconPath;
    public int trapLevel;
    public float trapDamage; //����������˺��ǻ����˺� ���������˺�������������Ӱ�� ��trap��mono�ű��д������
    public int trapScore;
    public TrapTriggerType triggerType;
    public TrapValidType validType;
    public float validTime = -1; //��Чʱ�� -1Ϊ����������Ч
    public List<TrapDeadType> deadTypeList;
    public float deadDelayTime;//�����ӳ�ʱ�� deadTypeList����TimeDelay����Ч
    public float placeTime;//��������Ҫ��ʱ��
    public bool hasCollider;//�Ƿ�����ײ��
    public string interactSoundName;
    public string pickSoundName;
    public string workSoundName;
    public string deadSoundName;
    public string interactParticleName;
    public string pickParticleName;
    public string workParticleName;
    public string deadParticleName;
    public float soundRange;
    public UseItemType UseItemType => UseItemType.trap;
    public string Id => id;
}

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