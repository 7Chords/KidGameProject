﻿/*using KidGame.Core;
using KidGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace KidGame.Core
{
    public class LineRenderScript : MonoBehaviour
    {
        #region 有的没的变量
        public LineRenderer lineRenderer; // 拖拽引用
        public Vector3 startPoint;       // 起始位置
        public Vector3 endPoint;         // 结束位置
        public float heightFactor = 1f;  // 抛物线高度因子
        public float heightHeightFactor = 20f; // 抛物线高度的高度因子
        public int resolution = 50;        // 轨迹点数量
        public AnimationCurve widthCurve;  // 宽度曲线
        public float currentWidth;
        public float startWidth = 0.05f;    // 默认宽度
        public float arrowStartWidth = 0.05f;     // 后面逐渐减小的宽度
        public float heightFactorLerpMin = 0.9f;
        public float heightFactorLerpMax = 1.2f;
        private Vector3[] points;
        private Vector3 lastFrameStartPositon;// 上一帧开始位置
        private Vector3 lastFrameEndPositon;// 上一帧结束位置
        #endregion

        private void Awake()
        {
            points = new Vector3[resolution];
            widthCurve = new AnimationCurve();
            DrawParabola();
        }
        private void Start()
        {
        }

        private void Update()
        {
            // 条件时实更新
            if(lastFrameStartPositon != startPoint || lastFrameEndPositon != endPoint)
                DrawParabola();
        }

        void DrawParabola()
        {
            lastFrameEndPositon = endPoint;
            lastFrameStartPositon = startPoint;
            // 瞎√8调了一下这个高度 好像还算能接受 效果还行
            // 这边是在算一个高度因子
            float distance = Vector3.Distance(startPoint, endPoint);
            float t = (float)distance / heightHeightFactor;
            t = Mathf.Clamp01(1 - t);
            heightFactor = Mathf.Lerp(heightFactorLerpMin, heightFactorLerpMax, t);
            // 设置线的轨迹点数
            lineRenderer.positionCount = resolution;
            if (points != null && points.Length > 0)
                Array.Clear(points, 0, points.Length);

            // 基础高度位置 取连线中点
            Vector3 controlPoint = (startPoint + endPoint) / 2f;
            // 最终高度位置 这个5其实也是个系数 需要的话可以自己写一个public的factor
            controlPoint += Vector3.up * heightFactor * 5;

            // 生成贝塞尔曲线上的点
            for (int i = 0; i < resolution; i++)
            {
                t = (float)i / (resolution - 1); // 归一化
                // 通过贝塞尔曲线函数 算出每个点的坐标
                points[i] = CalculateBezierPoint(t, startPoint, controlPoint, endPoint);

                // 这个curve变化永远是 线性的 不是立即设置的 想做到一个箭头效果是不可能的
                // 除非加很多很多的点 但是感觉会得不偿失
                if (t < 0.7f && t >= 0f) currentWidth = startWidth;
                else
                {
                    // 在最后30% 
                    // 从结尾宽度 开始下降到宽度为0 模拟一个箭头
                    currentWidth = Mathf.Lerp(arrowStartWidth, 0, (t - 0.7f) / 0.3f);
                }
                // 抛物线宽度曲线
                if(widthCurve != null) widthCurve.AddKey(t, currentWidth);
            }

            // 应用轨迹点
            lineRenderer.SetPositions(points);

            // 应用宽度曲线
            lineRenderer.widthCurve = widthCurve;
        }

        // 计算二次贝塞尔曲线上的点
        // 公公又式式
        Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            return uu * p0 + 2 * u * t * p1 + tt * p2;
        }


        public Vector3[] GetPoints()
        {
            // 返回数组的副本（创建新数组并复制值）
            Vector3[] copy = new Vector3[points.Length];
            Array.Copy(points, copy, points.Length);
            return copy;
        }
    }
}*/

using KidGame.Core;
using KidGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace KidGame.Core
{
    public class LineRenderScript : MonoBehaviour
    {
        #region 变量
        public LineRenderer lineRenderer; // 拖拽引用
        public Vector3 startPoint;       // 起始位置a
        public Vector3 endPoint;         // 结束位置
        public float heightFactor = 1f;  // 抛物线高度因子
        public float heightHeightFactor = 20f; // 抛物线高度的基准距离
        public int resolution = 30;      // 轨迹点数量（优化为30，平衡性能与精度）
        public AnimationCurve widthCurve;  // 宽度曲线
        public float currentWidth;
        public float startWidth = 0.1f;    // 默认宽度
        public float arrowStartWidth = 0.1f;     // 箭头起始宽度
        public float heightFactorLerpMin = 0.9f;  // 高度因子最小值
        public float heightFactorLerpMax = 1.2f;  // 高度因子最大值
        private Vector3[] points;
        private Vector3 lastFrameStartPositon;// 上一帧开始位置
        private Vector3 lastFrameEndPositon;// 上一帧结束位置
        #endregion

        private void Awake()
        {
            points = new Vector3[resolution];
            widthCurve = new AnimationCurve();
            // 初始化LineRenderer参数
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = resolution;
                lineRenderer.useWorldSpace = true; // 确保使用世界坐标
            }
            DrawParabola();
        }

        private void Update()
        {
            // 仅在起点/终点变化时更新曲线（减少计算）
            if (lastFrameStartPositon != startPoint || lastFrameEndPositon != endPoint)
                DrawParabola();
        }

        void DrawParabola()
        {
            lastFrameEndPositon = endPoint;
            lastFrameStartPositon = startPoint;

            // 计算水平距离（忽略Y轴，用于调整抛物线高度）
            Vector3 horizontalStart = new Vector3(startPoint.x, 0, startPoint.z);
            Vector3 horizontalEnd = new Vector3(endPoint.x, 0, endPoint.z);
            float horizontalDistance = Vector3.Distance(horizontalStart, horizontalEnd);

            // 动态计算高度因子（距离越远，高度因子越大，抛物线更自然）
            float t = Mathf.Clamp01(horizontalDistance / heightHeightFactor);
            heightFactor = Mathf.Lerp(heightFactorLerpMin, heightFactorLerpMax, 1 - t);

            // 计算控制点（匹配物理抛物线顶点）
            Vector3 midPoint = (startPoint + endPoint) / 2f; // 水平中点
            float apexHeight = heightFactor * (horizontalDistance / 4f); // 顶点高度（与物理轨迹匹配）
            Vector3 controlPoint = new Vector3(midPoint.x, startPoint.y + apexHeight, midPoint.z);

            // 清空宽度曲线（避免累积键值）
            //widthCurve.keys = new Keyframe[0];

            // 生成贝塞尔曲线上的点
            for (int i = 0; i < resolution; i++)
            {
                float tParam = (float)i / (resolution - 1); // 归一化参数
                points[i] = CalculateBezierPoint(tParam, startPoint, controlPoint, endPoint);

                // 计算宽度曲线（箭头效果）
                if (tParam < 0.7f)
                    currentWidth = startWidth;
                else
                    currentWidth = Mathf.Lerp(arrowStartWidth, 0, (tParam - 0.7f) / 0.3f);

                widthCurve.AddKey(tParam, currentWidth);
            }

            // 应用轨迹点和宽度曲线
            lineRenderer.SetPositions(points);
            lineRenderer.widthCurve = widthCurve;
        }

        // 计算二次贝塞尔曲线上的点
        Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            return uu * p0 + 2 * u * t * p1 + tt * p2;
        }

        public Vector3[] GetPoints()
        {
            // 返回数组副本，避免外部修改
            Vector3[] copy = new Vector3[points.Length];
            Array.Copy(points, copy, points.Length);
            return copy;
        }
    }
}