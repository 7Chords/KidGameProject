using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{

    /// <summary>
    /// ÏÝÚå·ÅÖÃ¼ì²âÆ÷
    /// </summary>
    public class TrapPlaceTriggerSender : MonoBehaviour
    {
        private TrapBase _trap;

        private List<Collider> colls;
        private void Start()
        {
            _trap = GetComponentInParent<TrapBase>();
            colls = new List<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_trap == null) return;
            if(other.gameObject.layer == LayerMask.NameToLayer("Front"))
            {
                _trap.SetCanPlaceState(false);
                colls.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_trap == null) return;
            if (other.gameObject.layer == LayerMask.NameToLayer("Front"))
            {
                colls.Remove(other);
                if (colls.Count == 0)
                {
                    _trap.SetCanPlaceState(true);
                }
            }
        }
    }
}
