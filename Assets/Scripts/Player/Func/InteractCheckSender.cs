using KidGame.Interface;
using KidGame.UI;
using System.Collections.Generic;
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
            IInteractive collInteractive = other.gameObject.GetComponent<IInteractive>();
            IPickable pickable = other.gameObject.GetComponent<IPickable>();
            if (collInteractive == null && pickable == null) return;
            bubbleStr = "";
            tmpActionTypeList.Clear();
            if(collInteractive != null)
            {
                player.AddInteractiveToList(collInteractive, Vector3.Distance(other.gameObject.transform.position, player.transform.position));
                tmpActionTypeList.Add(InputActionType.Interaction);
                bubbleStr += "交互";//TODO:根据不同的交互者调整合适的文本
            }
            if (pickable != null)
            {
                player.AddPickableToList(other.gameObject.GetComponent<IPickable>(),
                    Vector3.Distance(other.gameObject.transform.position, player.transform.position));
                tmpActionTypeList.Add(InputActionType.Pick);
                bubbleStr += bubbleStr.Length == 0?"回收":"/回收";
            }
            UIHelper.Instance.AddBubbleInfoToList(new BubbleInfo(ControlType.Keyborad, tmpActionTypeList,
                other.gameObject, player.gameObject, bubbleStr, other.GetComponent<MapEntity>()?.EntityName));
        }

        private void OnTriggerExit(Collider other)
        {
            IInteractive collInteractive = other.gameObject.GetComponent<IInteractive>();
            if(collInteractive != null)
            {
                player.RemoveInteractiveFromList(other.gameObject.GetComponent<IInteractive>());
            }
            IPickable pickable = other.gameObject.GetComponent<IPickable>();
            if (pickable != null)
            {
                player.RemovePickableFromList(other.gameObject.GetComponent<IPickable>());
            }
            UIHelper.Instance.RemoveBubbleInfoFromList(other.gameObject);
        }
    }
}