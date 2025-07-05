using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ���屻��������Ϣ������
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
            //����������
            if (other.gameObject.tag != "Enemy") return;
            //tip����ýҲ������ �����Ǻʹ�ý�������� ���Ƕ���������˵�Ǳ������� ��ý��triggerЧ�����Ǵ�������ı�������
            _trap.InteractPositive(other.gameObject);
        }
    }
}