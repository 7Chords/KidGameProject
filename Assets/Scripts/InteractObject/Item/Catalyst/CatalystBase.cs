using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ý����
/// </summary>
public class CatalystBase : MapItem
{
    [SerializeField]
    protected TrapBase _trap;
    public void Init()
    {

    }
    public override void InteractNegative()
    {
        if (_trap == null) return;
        _trap.TriggerByCatalyst(this);
    }

    public override void InteractPositive()
    {
    }

    public override void Pick()
    {
        
    }

    public virtual void SetTrap(TrapBase trap)
    {
        _trap = trap;
    }
}
