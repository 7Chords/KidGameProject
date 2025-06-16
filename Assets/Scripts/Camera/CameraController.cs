using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class CameraController : MonoBehaviour
    {
        public Transform Player;

        private Vector3 originalPos;
        private Vector3 playerMovement;

        private void Start()
        {
            originalPos = Player.transform.position;
        }

        private void Update()
        {
            playerMovement = Player.transform.position - originalPos;
            originalPos = Player.transform.position;
        }

        private void LateUpdate()
        {
            transform.position += playerMovement;
        }
    }
}