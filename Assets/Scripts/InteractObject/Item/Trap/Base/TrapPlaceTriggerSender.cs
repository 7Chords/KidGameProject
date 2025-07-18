using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// ������ü���������μ��汾��
    /// </summary>
    public class TrapPlaceTriggerSender : MonoBehaviour
    {
        private TrapBase _trap;

        [Header("�������")]
        [SerializeField] private float detectionRadius = 0.5f;
        [SerializeField] private LayerMask obstacleLayers;

        private bool _canPlace = true;
        private Collider[] hitColliders = new Collider[10];

        private void Start()
        {
            _trap = GetComponentInParent<TrapBase>();
            obstacleLayers = LayerMask.GetMask("Front", "Interactive");
        }

        private void Update()
        {
            if (_trap == null) return;

            // ������巶Χ�ڵ�������ײ��
            int hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                detectionRadius,
                hitColliders,
                obstacleLayers);

            // ���·���״̬
            bool newCanPlace = hitCount == 0;
            if (newCanPlace != _canPlace)
            {
                _canPlace = newCanPlace;
                _trap.SetCanPlaceState(_canPlace);
            }

            // ���ӻ���ⷶΧ�����ڱ༭������ʾ��
            Debug.DrawRay(transform.position, Vector3.up * detectionRadius, Color.blue * 0.5f);
        }

        // ���ӻ���ⷶΧ���ڱ༭������ʾ��
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _canPlace ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}