using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBase : MapItem
{
    public override void Interact()
    {
        
    }

    public override void Pick()
    {
        PlayerUtil.Instance.CallPlayerPickItem(this);
    }
}
