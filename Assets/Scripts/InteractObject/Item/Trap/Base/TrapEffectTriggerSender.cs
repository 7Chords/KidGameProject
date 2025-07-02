using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 陷阱被动触发信息发送器
    /// </summary>
    public class TrapEffectTriggerSender : MonoBehaviour
    {
        private TrapBase _trap;

        private void Start()
        {
            _trap = GetComponentInParent<TrapBase>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_trap == null) return;
            //被动触发了
            if (other.gameObject.tag != "Enemy") return;
            _trap.InteractNegative(other.gameObject);
        }
    }
}