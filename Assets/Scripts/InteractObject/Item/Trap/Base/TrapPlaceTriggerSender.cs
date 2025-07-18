using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 陷阱放置检测器（球形检测版本）
    /// </summary>
    public class TrapPlaceTriggerSender : MonoBehaviour
    {
        private TrapBase _trap;

        [Header("检测配置")]
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

            // 检测球体范围内的所有碰撞体
            int hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                detectionRadius,
                hitColliders,
                obstacleLayers);

            // 更新放置状态
            bool newCanPlace = hitCount == 0;
            if (newCanPlace != _canPlace)
            {
                _canPlace = newCanPlace;
                _trap.SetCanPlaceState(_canPlace);
            }

            // 可视化检测范围（仅在编辑器中显示）
            Debug.DrawRay(transform.position, Vector3.up * detectionRadius, Color.blue * 0.5f);
        }

        // 可视化检测范围（在编辑器中显示）
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _canPlace ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}