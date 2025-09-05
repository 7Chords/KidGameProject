using KidGame.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.core
{
    public class Telescope : OnHandWeaponBase
    {
        [SerializeField] private FocusLinesScript _m_focusLinesScript;// 聚焦线脚本引用
        private float distanceR = 15f;// 距离 通过配表配置
        private int layerIndex;
        private float halfMinAngel = 0; // 最大能超过的角度
        private float onceFoucsAddTime = 0.05f; // 单次照射增加的值
        protected override void Awake()
        {
            layerIndex = LayerMask.NameToLayer("NeedShadow_Enemy");
            Init();
        }
        protected override void Update()
        {
            // 如果聚焦到最小了 才开始判断是否有敌人被照射
            base.Update();
        }
        // 准备阶段逻辑 白天
        public override void _WeaponUseLogic()
        {
            if(GameLevelManager.Instance.GetCurrentPhase() == LevelPhase.Day 
                && _m_focusLinesScript.GetIsTimeToCheck())
            {
                DoInDay();
            }
            else if(GameLevelManager.Instance.GetCurrentPhase() == LevelPhase.Night)
            {
                DoInNight();
            }
           
        }

        private void DoInNight()
        {

        }
        private void DoInDay()
        {
            Collider[] hitColliders = Physics.OverlapSphere(
            PlayerController.Instance.transform.position,  // 判断位置中心
            distanceR,        // 半径
            1 << layerIndex
            );

            // 判断
            foreach (Collider i in hitColliders)
            {
                if (i.TryGetComponent(out EnemyController controller))
                {
                    if (isEnemyInFocusArea(i.transform.position))
                    {
                        controller.AddBeExposeTime(onceFoucsAddTime);
                        // 这里还需要交流一下怎么处理比较合适
                        //if(controller.GetBeExposeTime() > )
                    }
                }

            }
        }

        // 敌人是否在聚焦范围内  应该传入敌人位置信息
        private bool isEnemyInFocusArea(Vector3 enemyPositions)
        {
            // 基准线
            Vector3 tempVec3 = _m_focusLinesScript.GetMiddleLine();

            Vector3 playerAndEnemyDir = (enemyPositions - PlayerController.Instance.transform.position).normalized;
            // 两者之间的夹角
            float angleBetween = Vector3.Angle(tempVec3, playerAndEnemyDir);

            if (angleBetween <= halfMinAngel) return true;

            return false;
        }

        private void Init()
        {
            halfMinAngel = _m_focusLinesScript.GetHalfMinAngle();
        }
    }
}