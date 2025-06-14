using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����������Ϣ������
/// </summary>
public class CatalystPutSender : MonoBehaviour
{
    private TrapBase _trap;

    private void Start()
    {
        _trap = GetComponentInParent<TrapBase>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_trap == null) return;
        if (other.gameObject.tag != "Catalyst") return;
        CatalystBase catalyst = other.GetComponent<CatalystBase>();
        if (catalyst == null) return;
        catalyst.SetTrap(_trap);
        _trap.SetCatalyst(catalyst);
    }
}
