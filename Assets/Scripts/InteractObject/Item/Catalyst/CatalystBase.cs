using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ´¥Ã½»ùÀà
/// </summary>
public class CatalystBase : MapItem
{
    protected TrapBase _trap;
    public void Init()
    {

    }
    public override void InteractNegative()
    {
        
    }

    public override void InteractPositive()
    {
        if (_trap == null) return;
        _trap.TriggerByCatalyst(this);
    }

    public override void Pick()
    {
        
    }

    public virtual void SetTrap(TrapBase trap)
    {
        _trap = trap;
    }
}
