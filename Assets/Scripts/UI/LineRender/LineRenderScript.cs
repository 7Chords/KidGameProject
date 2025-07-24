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
        #region 有的没的变量
        public LineRenderer lineRenderer; // 拖拽引用
        public Vector3 startPoint;       // 起始位置
        public Vector3 endPoint;         // 结束位置
        public float heightFactor;  // 抛物线高度因子
        public float heightHeightFactor; // 抛物线高度的高度因子
        public int resolution = 100;        // 轨迹点数量
        public AnimationCurve widthCurve;  // 宽度曲线
        public float currentWidth;
        public float startWidth;    // 默认宽度
        public float arrowStartWidth;     // 后面逐渐减小的宽度
        public float heightFactorLerpMin = 0.9f;
        public float heightFactorLerpMax = 1.2f;
        public Vector3[] points;
        #endregion
        private void Start()
        {
            DrawParabola();
        }

        private void Update()
        {
            // 时实更新
            DrawParabola();
        }

        void DrawParabola()
        {
            // 瞎√8调了一下这个高度 好像还算能接受 效果还行
            // 这边是在算一个高度因子
            float distance = Vector3.Distance(startPoint, endPoint);
            float t = (float)distance / heightHeightFactor;
            t = Mathf.Clamp01(1 - t);
            heightFactor = Mathf.Lerp(heightFactorLerpMin, heightFactorLerpMax, t);
            if (startPoint == null || endPoint == null) return;
            // 设置线的轨迹点数
            lineRenderer.positionCount = resolution;
            if (points.Length > 0)
                Array.Clear(points, 0, points.Length);
            points = new Vector3[resolution];

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
                widthCurve.AddKey(t, currentWidth);
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
    }
}