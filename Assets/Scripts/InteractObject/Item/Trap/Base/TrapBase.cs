using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������
/// </summary>
public class TrapBase : MapItem
{
    [SerializeField]//������
    protected TrapData _trapData;

    #region ʱ����������ز���
    protected bool _isTimeValid;
    protected float _validTime;
    protected float _validTimer;
    #endregion

    [Header("��ý���÷�Χ")]
    [SerializeField] protected Collider _catalystPutCollider;

    [Header("�������÷�Χ")]
    [SerializeField] protected Collider _trapEffectCollider;

    private CatalystBase _catalyst;

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="trapData"></param>
    public virtual void Init(TrapData trapData)
    {
        _trapData = trapData;
        _isTimeValid = _trapData.trapTypeList.Contains(TrapType.Time_Valid);
        _validTime = _trapData.validTime;
        _validTimer = 0;
    }

    #region �����ӿڷ���ʵ��
    public override void InteractPositive() 
    {
        if (_trapData == null) return;
        //��Ҫ��ý
        if (_trapData.triggerType == TrapTriggerType.Catalyst) return;
        //���������ͽ���
        if (!_trapData.trapTypeList.Contains(TrapType.Positive))
            return;
        Trigger();
    }
    public override void InteractNegative()
    {
        if (_trapData == null) return;
        //��Ҫ��ý
        if (_trapData.triggerType == TrapTriggerType.Catalyst) return;
        //���������ͽ���
        if (!_trapData.trapTypeList.Contains(TrapType.Negative))
            return;
        Trigger();
    }
    public override void Pick()
    {
        PlayerUtil.Instance.CallPlayerPickItem(this);
    }

    #endregion

    #region ������
    /// <summary>
    /// ʱ���������ʱ����
    /// </summary>
    public virtual void TimeValidTick()
    {
        if (!_isTimeValid) return;
        if (GetValidState()) return;
        _validTimer += GlobalValue.GameDeltaTime;
    }

    /// <summary>
    /// �����Ƿ���Ч
    /// </summary>
    /// <returns></returns>
    public virtual bool GetValidState()
    {
        if (!_isTimeValid) return true;
        if (_validTimer >= _validTime) return true;
        return false;
    }

    /// <summary>
    /// ���ô�ý
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
    /// ���崥����Ч������
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
