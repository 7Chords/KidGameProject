using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerBaseData", menuName = "KidGameSO/Player/PlayerBaseData")]
public class PlayerBaseData : ScriptableObject
{
    [Header("�����ٶ�")]
    public float WalkSpeed;

    [Header("�����ٶ�")]
    public float RunSpeed;

    [Header("�������ֵ")]
    public int Hp;

    [Header("�������ֵ")]
    public float Sp;

    [Header("���γ�����ĵ�����ֵ")]
    public float DashStaminaOneTime;

    [Header("����һ�����ĵ�����ֵ")]
    public float RunStaminaPerSecond;

    [Header("ÿ��ָ�������")]
    public float StaminaRecoverPerSecond;

    [Header("�����ָ������ٰٷֱ��뿪����״̬")]
    public float RecoverThreshold;

    [Header("���������Ľ��ȣ�0��1��")]
    [Range(0,1)]
    public float StruggleAmountOneTime;

    [Header("����������޵�ʱ��")]
    public float StruggleInvulnerabilityDuration;

    [Header("���������Ч�б�")]
    public List<string> RandomDamgeSfxList;

    [Header("����������Ч")]
    public ParticleSystem DamagePartical;
}