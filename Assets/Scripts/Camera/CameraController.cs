using UnityEngine;

namespace KidGame.Core
{
    public class CameraController : Singleton<CameraController>
    {
        public Transform Player;
        public float smoothSpeed = 20f;
        private Vector3 offsetVec3 = Vector3.zero;
        private float initialDistance;

        public void Init()
        {
            if (Player == null)
                Player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (Player == null) return;
            initialDistance = Vector3.Distance(transform.position, Player.position);
        }

        public void Discard()
        {

        }

        private void LateUpdate()
        {
            if (Player == null) return;

            Vector3 desiredPosition = Player.position - transform.forward * initialDistance;
            
            transform.position = Vector3.Lerp(transform.position, desiredPosition + offsetVec3, Time.deltaTime * smoothSpeed);
        }
        // 设置相机偏移 offset为方向向量 t为倍数
        public void SetCamaraOffset(Vector3 offset, int t)
        {
            offsetVec3 = offset * t;
            offsetVec3.y = 0;
        }

        public void SetSmoothSpeed(float speed)
        {
            smoothSpeed = speed;
        }
        // 复位
        public void ResetCamaraOffset()
        {
            offsetVec3 = Vector3.zero;
        }

        public void ResetCamaraSmoothSpeed()
        {
            smoothSpeed = 20f;
        }
    }
}