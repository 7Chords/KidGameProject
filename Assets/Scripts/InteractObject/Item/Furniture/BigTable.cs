using KidGame.Interface;
using UnityEngine;
using System;
using System.Collections.Generic;
using KidGame.UI;

namespace KidGame.Core
{
    [Serializable]
    public class BigTableTeleportGroup
    {
        public Transform OutPoint;
        public Transform InPoint;
    }

    public class BigTable : MapFurniture, IInteractive
    {
        private bool isPlayerUnderTable;
        public List<BigTableTeleportGroup> TeleportPointGroupList;

        public void InteractNegative(GameObject interactor) { }

        public void InteractPositive(GameObject interactor)
        {

            Vector3 targetPoint = Vector3.zero;
            float maxDistance = 999;
            float curDistance = 0;
            foreach(var group in TeleportPointGroupList)
            {
                if(!isPlayerUnderTable)
                {
                    curDistance = Vector3.Distance(PlayerController.Instance.transform.position, group.InPoint.position);
                    if (curDistance < maxDistance)
                    {
                        maxDistance = curDistance;
                        targetPoint = group.InPoint.position;
                    }
                }
                else
                {
                    curDistance = Vector3.Distance(PlayerController.Instance.transform.position, group.OutPoint.position);
                    if (curDistance < maxDistance)
                    {
                        maxDistance = curDistance;
                        targetPoint = group.OutPoint.position;
                    }

                }
            }

            targetPoint = new Vector3(targetPoint.x, PlayerController.Instance.transform.position.y, targetPoint.z);

            PlayerController.Instance.transform.position = targetPoint;

            isPlayerUnderTable = !isPlayerUnderTable;

            MsgCenter.SendMsg(MsgConst.ON_PLAYER_HIDE_CHG, isPlayerUnderTable);

            if(isPlayerUnderTable)
                UIHelper.Instance.UpdateBubbleContent(gameObject,"×ê³ö");
            else
                UIHelper.Instance.UpdateBubbleContent(gameObject, "½»»¥");

        }

    }
}
