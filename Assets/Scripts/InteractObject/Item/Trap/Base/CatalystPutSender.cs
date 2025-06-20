using KidGame.Core;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 触发放置信息发射器
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

        private void OnTriggerExit(Collider other)
        {
            if (_trap == null) return;
            
            if (other.gameObject.tag != "Catalyst") return;
            
            CatalystBase catalyst = other.GetComponent<CatalystBase>();
            catalyst.SetTrap(null);
            _trap.SetCatalyst(null);
        }
    }
}