using KidGame.UI;
using UnityEngine;

namespace KidGame.Core
{
    public partial class PlayerController
    {
        /// <summary>
        /// 玩家使用物品
        /// </summary>
        public void PlayerUseItem()
        {
            if (!CanPlayerUseItem()) return;

            ISlotInfo currentUseItem = PlayerBag.Instance.GetSelectedQuickAccessItem();
            if (currentUseItem == null) return;
            UseItemType itemType = currentUseItem.ItemData.UseItemType;
            //允许变走边使用武器
            if (playerInfo.PlayerState != PlayerState.Idle && itemType != UseItemType.weapon)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("当前状态不可布置陷阱", transform.position));
                return;
            }
            if (currentUseItem is WeaponSlotInfo weaponItem)
            {
                
            }
            ChangeState(PlayerState.Use);
        }

        private bool CanPlayerUseItem()
        {
            if(playerInfo.IsHide)
            {
                return false;
            }
            return true;
        }

        public void TryUseWeaponUseLongPress()
        {
            //Logic
        }

        private void OnItemSelected(object[] objs)
        {
            ISlotInfo slotInfo;
            if (objs == null)
                slotInfo = null;
            else
                slotInfo = objs[0] as ISlotInfo;

            if (slotInfo == null || slotInfo.ItemData is MaterialData || slotInfo.ItemData is FoodData)
            {
                DestoryCurrentTrapPreview();
                DiscardWeaponAndWeaponData();
                return;
            }
            if(slotInfo.ItemData is TrapData trapData)
            {
                SpawnSelectTrapPreview(trapData);
            }
            else if(slotInfo.ItemData is WeaponData weaponData)
            {
                if (playerInfo.CurrentWeaponData != null && weaponData.id == playerInfo.CurrentWeaponData.id) return;
                // 消耗品  远程
                if (weaponData.useType == 0 && weaponData.weaponType == 1)
                {

                    // 如果不是重复的 销毁现在的 取得新的
                    // 不销毁 只让引用为空 销毁在逻辑里做了 否则会有一些问题
                    DiscardWeaponAndWeaponData();
                    playerInfo.CurWeaponGO = SpawnThrowWeapon(
                        weaponData,
                        this.transform.rotation
                        );
                }
                //可多次使用  近战
                else if(weaponData.useType == 1 && weaponData.weaponType == 0)
                {
                    DiscardWeaponAndWeaponData();
                    playerInfo.CurWeaponGO = SpawnOnHandWeapon(
                        weaponData,
                        SpawnAndUseOnHandWeaponPoint.rotation
                        );
                }
            }
        }

        public void SetCurrentWeaponData(WeaponData _weaponData)
        {
            playerInfo.CurrentWeaponData = _weaponData;
        }

        public GameObject GetCurWeapon()
        {
            return playerInfo.CurWeaponGO;
        }

        public void SetWeaponAndWeaponDataReference2Null()
        {
            Debug.Log("YES");
            playerInfo.CurWeaponGO = null;
            playerInfo.CurrentWeaponData = null;
        }

        public void DiscardWeaponAndWeaponData()
        {
            if(playerInfo.CurWeaponGO != null && playerInfo.CurrentWeaponData != null)
            {
                Destroy(playerInfo.CurWeaponGO);
            }
            playerInfo.CurWeaponGO = null;
            playerInfo.CurrentWeaponData = null;
        }

        /// <summary>
        /// 生成预览的陷阱
        /// </summary>
        /// <param name="obj"></param>
        private void SpawnSelectTrapPreview(TrapData trapData)
        {
            DestoryCurrentTrapPreview();
            playerInfo.CurPreviewTrapGO = TrapFactory.CreatePreview(trapData, PlaceTrapPoint.position, transform);
        }

        private void DestoryCurrentTrapPreview()
        {
            //在放置陷阱时切换 立刻结束放置状态
            if (playerInfo.PlayerState == PlayerState.Use)
            {
                ChangeState(PlayerState.Idle);
                UIHelper.Instance.DestoryCircleProgress(GlobalValue.CIRCLE_PROGRESS_PLACE_TRAP);
            }
            if (playerInfo.CurPreviewTrapGO) Destroy(playerInfo.CurPreviewTrapGO);
        }

        //生成在手上的武器 但是不执行逻辑
        public GameObject SpawnThrowWeapon(WeaponData weaponData, Quaternion rotation)
        {
            if (SpawnAndUseThrowWeaponPoint == null) return null;

            GameObject newWeapon = WeaponFactory.CreateEntity(
                weaponData
                , SpawnAndUseThrowWeaponPoint.position
                , this.transform);

            if (newWeapon != null)
            {
                newWeapon.transform.rotation = rotation;
            }
            // 在手上的话 启用渲染
            LineRenderer lineRenderer = newWeapon.GetComponent<LineRenderer>();
            if(lineRenderer != null) { lineRenderer.enabled = true; }
            return newWeapon;
        }

        public GameObject SpawnOnHandWeapon(WeaponData weaponData, Quaternion rotation)
        {
            if (SpawnAndUseThrowWeaponPoint == null) return null;

            GameObject newWeapon = WeaponFactory.CreateEntity(
                weaponData
                , SpawnAndUseThrowWeaponPoint.position
                , this.transform);

            if (newWeapon != null)
            {
                newWeapon.transform.rotation = rotation;
            }

            return newWeapon;
        }

        public bool GetCanPlaceTrap()
        {
            if (playerInfo.CurPreviewTrapGO == null) return false;
            return playerInfo.CurPreviewTrapGO.GetComponentInParent<TrapBase>().CanPlaceTrap;
        }
    }
}