using KidGame.UI;
using UnityEngine;

namespace KidGame.Core
{
    public partial class PlayerController
    {
        /// <summary>
        /// ���ʹ����Ʒ
        /// </summary>
        public void PlayerUseItem()
        {
            if (!CanPlayerUseItem()) return;

            ISlotInfo currentUseItem = PlayerBag.Instance.GetSelectedQuickAccessItem();
            if (currentUseItem == null) return;
            UseItemType itemType = currentUseItem.ItemData.UseItemType;
            //������߱�ʹ������
            if (playerInfo.PlayerState != PlayerState.Idle && itemType != UseItemType.weapon)
            {
                UIHelper.Instance.ShowOneTip(new TipInfo("��ǰ״̬���ɲ�������", transform.position));
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
                // ����Ʒ  Զ��
                if (weaponData.useType == 0 && weaponData.weaponType == 1)
                {

                    // ��������ظ��� �������ڵ� ȡ���µ�
                    // ������ ֻ������Ϊ�� �������߼������� �������һЩ����
                    DiscardWeaponAndWeaponData();
                    playerInfo.CurWeaponGO = SpawnThrowWeapon(
                        weaponData,
                        this.transform.rotation
                        );
                }
                //�ɶ��ʹ��  ��ս
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
        /// ����Ԥ��������
        /// </summary>
        /// <param name="obj"></param>
        private void SpawnSelectTrapPreview(TrapData trapData)
        {
            DestoryCurrentTrapPreview();
            playerInfo.CurPreviewTrapGO = TrapFactory.CreatePreview(trapData, PlaceTrapPoint.position, transform);
        }

        private void DestoryCurrentTrapPreview()
        {
            //�ڷ�������ʱ�л� ���̽�������״̬
            if (playerInfo.PlayerState == PlayerState.Use)
            {
                ChangeState(PlayerState.Idle);
                UIHelper.Instance.DestoryCircleProgress(GlobalValue.CIRCLE_PROGRESS_PLACE_TRAP);
            }
            if (playerInfo.CurPreviewTrapGO) Destroy(playerInfo.CurPreviewTrapGO);
        }

        //���������ϵ����� ���ǲ�ִ���߼�
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
            // �����ϵĻ� ������Ⱦ
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