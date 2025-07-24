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
        #region �е�û�ı���
        public LineRenderer lineRenderer; // ��ק����
        public Vector3 startPoint;       // ��ʼλ��
        public Vector3 endPoint;         // ����λ��
        public float heightFactor;  // �����߸߶�����
        public float heightHeightFactor; // �����߸߶ȵĸ߶�����
        public int resolution = 100;        // �켣������
        public AnimationCurve widthCurve;  // �������
        public float currentWidth;
        public float startWidth;    // Ĭ�Ͽ��
        public float arrowStartWidth;     // �����𽥼�С�Ŀ��
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
            // ʱʵ����
            DrawParabola();
        }

        void DrawParabola()
        {
            // Ϲ��8����һ������߶� �������ܽ��� Ч������
            // ���������һ���߶�����
            float distance = Vector3.Distance(startPoint, endPoint);
            float t = (float)distance / heightHeightFactor;
            t = Mathf.Clamp01(1 - t);
            heightFactor = Mathf.Lerp(heightFactorLerpMin, heightFactorLerpMax, t);
            if (startPoint == null || endPoint == null) return;
            // �����ߵĹ켣����
            lineRenderer.positionCount = resolution;
            if (points.Length > 0)
                Array.Clear(points, 0, points.Length);
            points = new Vector3[resolution];

            // �����߶�λ�� ȡ�����е�
            Vector3 controlPoint = (startPoint + endPoint) / 2f;
            // ���ո߶�λ�� ���5��ʵҲ�Ǹ�ϵ�� ��Ҫ�Ļ������Լ�дһ��public��factor
            controlPoint += Vector3.up * heightFactor * 5;

            // ���ɱ����������ϵĵ�
            for (int i = 0; i < resolution; i++)
            {
                t = (float)i / (resolution - 1); // ��һ��
                // ͨ�����������ߺ��� ���ÿ���������
                points[i] = CalculateBezierPoint(t, startPoint, controlPoint, endPoint);

                // ���curve�仯��Զ�� ���Ե� �����������õ� ������һ����ͷЧ���ǲ����ܵ�
                // ���ǼӺܶ�ܶ�ĵ� ���Ǹо���ò���ʧ
                if (t < 0.7f && t >= 0f) currentWidth = startWidth;
                else
                {
                    // �����30% 
                    // �ӽ�β��� ��ʼ�½������Ϊ0 ģ��һ����ͷ
                    currentWidth = Mathf.Lerp(arrowStartWidth, 0, (t - 0.7f) / 0.3f);
                }
                // �����߿������
                widthCurve.AddKey(t, currentWidth);
            }

            // Ӧ�ù켣��
            lineRenderer.SetPositions(points);

            // Ӧ�ÿ������
            lineRenderer.widthCurve = widthCurve;
        }

        // ������α����������ϵĵ�
        // ������ʽʽ
        Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            return uu * p0 + 2 * u * t * p1 + tt * p2;
        }
    }
}