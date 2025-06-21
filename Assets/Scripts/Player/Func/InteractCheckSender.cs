using KidGame.Interface;
using UnityEngine;

namespace KidGame.Core
{
    public class InteractCheckSender : MonoBehaviour
    {
        private PlayerController player;

        private void Awake()
        {
            player = GetComponentInParent<PlayerController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Interactive")) return;
            player.AddInteractiveToList(other.gameObject.GetComponent<IInteractive>(),
                Vector3.Distance(other.gameObject.transform.position, player.transform.position));
            IRecyclable recyclable = other.gameObject.GetComponent<IRecyclable>();
            if(recyclable != null)
            {
                player.AddIRecyclableToList(other.gameObject.GetComponent<IRecyclable>(),
                    Vector3.Distance(other.gameObject.transform.position, player.transform.position));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Interactive")) return;
            player.RemoveInteractiveFromList(other.gameObject.GetComponent<IInteractive>());
            IRecyclable recyclable = other.gameObject.GetComponent<IRecyclable>();
            if (recyclable != null)
            {
                player.RemoveRecyclableFromList(other.gameObject.GetComponent<IRecyclable>());
            }
        }
    }
}
