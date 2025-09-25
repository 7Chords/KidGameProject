using System.Collections.Generic;
using KidGame.Interface;
using UnityEngine;

namespace KidGame.Core
{
    public partial class PlayerController
    {
        /// <summary>
        /// 玩家交互
        /// </summary>
        public void PlayerInteraction()
        {
            if (playerInfo.InteractiveDict == null || playerInfo.InteractiveDict.Count == 0) return;
            GetClosestInteractive()?.InteractPositive(gameObject);
        }

        public void PlayerPick()
        {
            if (playerInfo.PickableDict == null || playerInfo.PickableDict.Count == 0) return;
            GetClosestPickable()?.Pick();
        }

        /// <summary>
        /// 添加到可交互列表
        /// </summary>
        /// <param name="interactive"></param>
        public void AddInteractiveToList(IInteractive interactive, float distance)
        {
            if (playerInfo.InteractiveDict == null) return;
            if (playerInfo.InteractiveDict.ContainsKey(interactive)) return;
            playerInfo.InteractiveDict.Add(interactive, distance);
        }

        /// <summary>
        /// 从可交互列表中移除
        /// </summary>
        /// <param name="interactive"></param>
        public void RemoveInteractiveFromList(IInteractive interactive)
        {
            if (playerInfo.InteractiveDict == null) return;
            if (!playerInfo.InteractiveDict.ContainsKey(interactive)) return;
            playerInfo.InteractiveDict.Remove(interactive);
        }

        /// <summary>
        /// 获得最近的可交互者
        /// </summary>
        private IInteractive GetClosestInteractive()
        {
            float min = 999;
            IInteractive closestInteractive = null;
            foreach (var pair in playerInfo.InteractiveDict)
            {
                if (pair.Value < min)
                {
                    min = pair.Value;
                    closestInteractive = pair.Key;
                }
            }

            return closestInteractive;
        }

        /// <summary>
        /// 添加到可回收列表
        /// </summary>
        /// <param name="interactive"></param>
        public void AddPickableToList(IPickable pickable, float distance)
        {
            if (playerInfo.PickableDict == null) return;
            if (playerInfo.PickableDict.ContainsKey(pickable)) return;
            playerInfo.PickableDict.Add(pickable, distance);
        }

        /// <summary>
        /// 从可回收列表中移除
        /// </summary>
        /// <param name="interactive"></param>
        public void RemovePickableFromList(IPickable pickable)
        {
            if (playerInfo.PickableDict == null) return;
            if (!playerInfo.PickableDict.ContainsKey(pickable)) return;
            playerInfo.PickableDict.Remove(pickable);
        }

        /// <summary>
        /// 获得最近的可拾取者
        /// </summary>
        private IPickable GetClosestPickable()
        {
            float min = 999;
            IPickable closestIPickable = null;
            foreach (var pair in playerInfo.PickableDict)
            {
                if (pair.Value < min)
                {
                    min = pair.Value;
                    closestIPickable = pair.Key;
                }
            }

            return closestIPickable;
        }
    }
}