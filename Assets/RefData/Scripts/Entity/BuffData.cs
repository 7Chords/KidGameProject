using System;
using System.Collections.Generic;
using UnityEngine;

//buff����µĲ���ʱ��ʱ����·�ʽ
public enum BuffAddStackUpdateType
{
    Add,//��ԭ��ʱ��������
    Replace,//�滻Ϊ�µ�ʱ�䣨ˢ�£�
    Keep,//ʱ�䱣�ֲ���
}

//buff�Ƴ�ʱ�ĸ��·�ʽ
public enum BuffRemoveStackUpdateType
{
    Clear,//��������buff���� 
    Reduce,//����buff����
}

[Serializable]
public class BuffData
{
    //������Ϣ
    public string id;

    public string buffName;

    public string description;

    public string iconPath;

    public int priority;//���ȼ� �����ж���������ִ����һ��

    public int maxStack;//���ɵ�������

    public List<string> tags;//buff��һЩ������� �˺��͵� 

    //ʱ����Ϣ

    public bool isForever;//�Ƿ�������buff

    public float duration;//����ʱ��

    public float tickTime;//�������ʱ��

    //���·�ʽ

    public BuffAddStackUpdateType buffAddStackUpdateTime;

    public BuffRemoveStackUpdateType buffRemoveStackUpdate;

    //�����ص���
    public string onCreateModulePath;

    public string onRemoveModulePath;

    public string onTickModulePath;

    //�˺��ص���

    public string onHitModulePath;

    public string onBeHurtModulePath;

    //etc...
}

