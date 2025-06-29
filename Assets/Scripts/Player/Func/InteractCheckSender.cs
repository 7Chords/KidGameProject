using KidGame.Interface;
using KidGame.UI;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace KidGame.Core
{
    public class InteractCheckSender : MonoBehaviour
    {
        private PlayerController player;

        private List<InputActionType> tmpActionTypeList;

        private string bubbleStr;
        private void Awake()
        {
            player = GetComponentInParent<PlayerController>();

            tmpActionTypeList = new List<InputActionType>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Interactive")) return;
            bubbleStr = "";
            tmpActionTypeList.Clear();
            IInteractive collInteractive = other.gameObject.GetComponent<IInteractive>();
            player.AddInteractiveToList(collInteractive,Vector3.Distance(other.gameObject.transform.position, player.transform.position));
            tmpActionTypeList.Add(InputActionType.Interaction);
            bubbleStr += "交互";//TODO:根据不同的交互者调整合适的文本
            IRecyclable recyclable = other.gameObject.GetComponent<IRecyclable>();
            if (recyclable != null)
            {
                player.AddIRecyclableToList(other.gameObject.GetComponent<IRecyclable>(),
                    Vector3.Distance(other.gameObject.transform.position, player.transform.position));
                tmpActionTypeList.Add(InputActionType.Recycle);
                bubbleStr += "/回收";
            }
            BubbleManager.Instance.AddBubbleInfoToList(new BubbleInfo(ControlType.Keyborad, tmpActionTypeList,
                other.gameObject, player.gameObject, bubbleStr, collInteractive.itemName));
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
            BubbleManager.Instance.RemoveBubbleInfoFromList(other.gameObject);
        }
    }
}