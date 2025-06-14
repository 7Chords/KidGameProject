using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 陷阱基类
/// </summary>
public class TrapBase : MapItem
{
    [SerializeField]//测试用
    protected TrapData _trapData;

    #region 时间型陷阱相关参数
    protected bool _isTimeValid;
    protected float _validTime;
    protected float _validTimer;
    #endregion

    [Header("触媒放置范围")]
    [SerializeField] protected Collider _catalystPutCollider;

    [Header("陷阱作用范围")]
    [SerializeField] protected Collider _trapEffectCollider;

    private CatalystBase _catalyst;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="trapData"></param>
    public virtual void Init(TrapData trapData)
    {
        _trapData = trapData;
        _isTimeValid = _trapData.trapTypeList.Contains(TrapType.Time_Valid);
        _validTime = _trapData.validTime;
        _validTimer = 0;
    }

    #region 交互接口方法实现
    public override void InteractPositive() 
    {
        if (_trapData == null) return;
        //需要触媒
        if (_trapData.triggerType == TrapTriggerType.Catalyst) return;
        //主动触发型交互
        if (!_trapData.trapTypeList.Contains(TrapType.Positive))
            return;
        Trigger();
    }
    public override void InteractNegative()
    {
        if (_trapData == null) return;
        //需要触媒
        if (_trapData.triggerType == TrapTriggerType.Catalyst) return;
        //主动触发型交互
        if (!_trapData.trapTypeList.Contains(TrapType.Negative))
            return;
        Trigger();
    }
    public override void Pick()
    {
        PlayerUtil.Instance.CallPlayerPickItem(this);
    }

    #endregion

    #region 功能性
    /// <summary>
    /// 时间型陷阱计时更新
    /// </summary>
    public virtual void TimeValidTick()
    {
        if (!_isTimeValid) return;
        if (GetValidState()) return;
        _validTimer += GlobalValue.GameDeltaTime;
    }

    /// <summary>
    /// 陷阱是否有效
    /// </summary>
    /// <returns></returns>
    public virtual bool GetValidState()
    {
        if (!_isTimeValid) return true;
        if (_validTimer >= _validTime) return true;
        return false;
    }

    /// <summary>
    /// 设置触媒
    /// </summary>
    /// <param name="catalyst"></param>
    public void SetCatalyst(CatalystBase catalyst)
    {
        _catalyst = catalyst;
    }

    public void TriggerByCatalyst()
    {
        if (_trapData == null || _trapData.triggerType != TrapTriggerType.Catalyst) return;
        Trigger();
    }

    /// <summary>
    /// 陷阱触发的效果代码
    /// </summary>
    public virtual void Trigger()
    {
        
    }

    #endregion



    #region Gizom
    private void OnDrawGizmos()
    {
        
    }
    #endregion
}
