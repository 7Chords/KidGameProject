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
            //tip：触媒也是陷阱 敌人是和触媒主动交互 但是对于陷阱来说是被动交互 触媒的trigger效果就是触发陷阱的被动交互
            _trap.InteractPositive(other.gameObject);
        }
    }
}