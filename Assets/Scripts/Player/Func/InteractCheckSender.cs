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
            bubbleStr = "";
            tmpActionTypeList.Clear();
            if (other.gameObject.layer != LayerMask.NameToLayer("Interactive")) return;
            player.AddInteractiveToList(other.gameObject.GetComponent<IInteractive>(),
                Vector3.Distance(other.gameObject.transform.position, player.transform.position));
            //加入要显示的键位提示
            tmpActionTypeList.Add(InputActionType.Interaction);
            bubbleStr += "交互";
            IRecyclable recyclable = other.gameObject.GetComponent<IRecyclable>();
            if(recyclable != null)
            {
                player.AddIRecyclableToList(other.gameObject.GetComponent<IRecyclable>(),
                    Vector3.Distance(other.gameObject.transform.position, player.transform.position));
                tmpActionTypeList.Add(InputActionType.Recycle);
                bubbleStr += "/回收";
            }
            BubbleManager.Instance.AddBubbleInfoToList(new BubbleInfo(ControlType.Keyborad, tmpActionTypeList,
                other.gameObject, player.gameObject, bubbleStr));
        }

        private void OnTriggerExit(Collider other)
        {
            bubbleStr = "";
            tmpActionTypeList.Clear();
            if (other.gameObject.layer != LayerMask.NameToLayer("Interactive")) return;
            player.RemoveInteractiveFromList(other.gameObject.GetComponent<IInteractive>());
            IRecyclable recyclable = other.gameObject.GetComponent<IRecyclable>();
            if (recyclable != null)
            {
                player.RemoveRecyclableFromList(other.gameObject.GetComponent<IRecyclable>());
            }
            //只要传入的creator对 其他数据无所谓 因为只需要判断该creator的气泡然后删掉
            BubbleManager.Instance.RemoveBubbleInfoFromList(new BubbleInfo(ControlType.Keyborad, tmpActionTypeList,
                other.gameObject, player.gameObject, bubbleStr));
        }
    }
}
