using UnityEngine;

namespace KidGame.Core
{
    public class CameraController : Singleton<CameraController>
    {
        public Transform Player;
        public float smoothSpeed = 5f;

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
            
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        }
    }
}