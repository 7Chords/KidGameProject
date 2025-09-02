using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerBaseData", menuName = "KidGameSO/Player/PlayerBaseData")]
public class PlayerBaseData : ScriptableObject
{
    [Header("行走速度")]
    public float WalkSpeed;

    [Header("奔跑速度")]
    public float RunSpeed;

    [Header("最大生命值")]
    public int Hp;

    [Header("最大体力值")]
    public float Sp;

    [Header("单次冲刺消耗的体力值")]
    public float DashStaminaOneTime;

    [Header("奔跑一秒消耗的体力值")]
    public float RunStaminaPerSecond;

    [Header("每秒恢复的体力")]
    public float StaminaRecoverPerSecond;

    [Header("体力恢复到多少百分比离开力竭状态")]
    public float RecoverThreshold;

    [Header("单次挣扎的进度（0，1）")]
    [Range(0,1)]
    public float StruggleAmountOneTime;

    [Header("挣扎过后的无敌时间")]
    public float StruggleInvulnerabilityDuration;

    [Header("受伤随机音效列表")]
    public List<string> RandomDamgeSfxList;

    [Header("受伤粒子特效")]
    public ParticleSystem DamagePartical;
}