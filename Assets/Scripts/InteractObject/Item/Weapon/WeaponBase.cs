using KidGame.Interface;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KidGame.Core
{

    public abstract class WeaponBase : MapItem, IInteractive
    {
        protected bool isOnHand = true;
        // 数据都去配表拿吧
        private WeaponData _weaponData;
        // 自己的引用
        protected GameObject self;
        
        // 曲线引用
        protected LineRenderer lineRenderer;

        // 脚本 用于赋值
        protected LineRenderScript lineRenderScript;

        public override string EntityName { get => _weaponData.name;}
        public WeaponData weaponData
        {
            get
            {
                return _weaponData;
            }
        }

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
            // 生成不在这里做 这里只做逻辑
            self = this.gameObject;
        }

        protected virtual void Update()
        {
            if (!isOnHand) WeaponUseLogic();
            else
            {
                SetStartPoint();
                SetEndPoint();
            }
        }

        // 设置抛物线终点
        private void SetEndPoint()
        {
            if(lineRenderScript != null)
            {
                Vector3 curVector3 = MouseRaycaster.Instance.GetMousePosi();
                if(curVector3 != Vector3.zero)
                {
                    lineRenderScript.endPoint = curVector3;
                }
            }
        }
        // 设置抛物线起点
        private void SetStartPoint()
        {
            if (lineRenderScript != null)
            {
               lineRenderScript.startPoint = this.transform.position;
            }
        }
        public virtual void InitWeaponData(WeaponData weaponData)
        {
            _weaponData = weaponData;
        }
        protected virtual void InitLineRender()
        {
            // 把曲线脚本挂上
            lineRenderer = GetComponent<LineRenderer>();
            // 把控制曲线的脚本挂上
            lineRenderScript = GetComponent<LineRenderScript>();
            lineRenderScript.lineRenderer = lineRenderer;
            lineRenderScript.startPoint = 
                PlayerController.Instance.transform.position
                + PlayerController.Instance.transform.forward;
            //TODO：在玩家组件上加一个物体 通过这个空物体来设置发射起点,否则有时序问题
            lineRenderScript.endPoint = MouseRaycaster.Instance.GetMousePosi();
        }

        /// <summary>
        /// 捡起物体的时候调用
        /// </summary>
        public override void Pick()
        {
            //他是通过Trigger类型Colider来增加物体的
            //如果捡起来的时候就移除
            PlayerController.Instance.RemovePickableFromList(this);
            //目前这个回调只注册了一个添加物品到背包的函数
            PlayerUtil.Instance.CallPlayerPickItem(weaponData.id,UseItemType.weapon);
            //这个气泡也在上面这个回调加到列表里了   在这个地方移除
            UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);

            //TODO:武器捡起来可能还有什么要触发的东西 待定

            UIHelper.Instance.ShowOneTip(new TipInfo("获得了" + EntityName + "×1", gameObject));

            Destroy(gameObject);
        }

        public void SetOnHandOrNot(bool onHand)
        {
            isOnHand = onHand;
        }
        public bool GetOnHandOrNot()
        {
            return isOnHand;
        }
        public virtual void InteractPositive(GameObject interactor)
        {
            //To Do:武器可能会被用来附魔
        }
 
        public virtual void InteractNegative(GameObject interactor)
        {

        }

        public abstract void WeaponUseLogic();
    }

}
