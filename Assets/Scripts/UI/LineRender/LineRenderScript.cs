/*using KidGame.Core;
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
        public float heightFactor = 1f;  // �����߸߶�����
        public float heightHeightFactor = 20f; // �����߸߶ȵĸ߶�����
        public int resolution = 50;        // �켣������
        public AnimationCurve widthCurve;  // �������
        public float currentWidth;
        public float startWidth = 0.05f;    // Ĭ�Ͽ��
        public float arrowStartWidth = 0.05f;     // �����𽥼�С�Ŀ��
        public float heightFactorLerpMin = 0.9f;
        public float heightFactorLerpMax = 1.2f;
        private Vector3[] points;
        private Vector3 lastFrameStartPositon;// ��һ֡��ʼλ��
        private Vector3 lastFrameEndPositon;// ��һ֡����λ��
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
            // ����ʱʵ����
            if(lastFrameStartPositon != startPoint || lastFrameEndPositon != endPoint)
                DrawParabola();
        }

        void DrawParabola()
        {
            lastFrameEndPositon = endPoint;
            lastFrameStartPositon = startPoint;
            // Ϲ��8����һ������߶� �������ܽ��� Ч������
            // ���������һ���߶�����
            float distance = Vector3.Distance(startPoint, endPoint);
            float t = (float)distance / heightHeightFactor;
            t = Mathf.Clamp01(1 - t);
            heightFactor = Mathf.Lerp(heightFactorLerpMin, heightFactorLerpMax, t);
            // �����ߵĹ켣����
            lineRenderer.positionCount = resolution;
            if (points != null && points.Length > 0)
                Array.Clear(points, 0, points.Length);

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
                if(widthCurve != null) widthCurve.AddKey(t, currentWidth);
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


        public Vector3[] GetPoints()
        {
            // ��������ĸ��������������鲢����ֵ��
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
        #region ����
        public LineRenderer lineRenderer; // ��ק����
        public Vector3 startPoint;       // ��ʼλ��
        public Vector3 endPoint;         // ����λ��
        public float heightFactor = 1f;  // �����߸߶�����
        public float heightHeightFactor = 20f; // �����߸߶ȵĻ�׼����
        public int resolution = 30;      // �켣���������Ż�Ϊ30��ƽ�������뾫�ȣ�
        public AnimationCurve widthCurve;  // �������
        public float currentWidth;
        public float startWidth = 0.1f;    // Ĭ�Ͽ��
        public float arrowStartWidth = 0.1f;     // ��ͷ��ʼ���
        public float heightFactorLerpMin = 0.9f;  // �߶�������Сֵ
        public float heightFactorLerpMax = 1.2f;  // �߶��������ֵ
        private Vector3[] points;
        private Vector3 lastFrameStartPositon;// ��һ֡��ʼλ��
        private Vector3 lastFrameEndPositon;// ��һ֡����λ��
        #endregion

        private void Awake()
        {
            points = new Vector3[resolution];
            widthCurve = new AnimationCurve();
            // ��ʼ��LineRenderer����
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = resolution;
                lineRenderer.useWorldSpace = true; // ȷ��ʹ����������
            }
            DrawParabola();
        }

        private void Update()
        {
            // �������/�յ�仯ʱ�������ߣ����ټ��㣩
            if (lastFrameStartPositon != startPoint || lastFrameEndPositon != endPoint)
                DrawParabola();
        }

        void DrawParabola()
        {
            lastFrameEndPositon = endPoint;
            lastFrameStartPositon = startPoint;

            // ����ˮƽ���루����Y�ᣬ���ڵ��������߸߶ȣ�
            Vector3 horizontalStart = new Vector3(startPoint.x, 0, startPoint.z);
            Vector3 horizontalEnd = new Vector3(endPoint.x, 0, endPoint.z);
            float horizontalDistance = Vector3.Distance(horizontalStart, horizontalEnd);

            // ��̬����߶����ӣ�����ԽԶ���߶�����Խ�������߸���Ȼ��
            float t = Mathf.Clamp01(horizontalDistance / heightHeightFactor);
            heightFactor = Mathf.Lerp(heightFactorLerpMin, heightFactorLerpMax, 1 - t);

            // ������Ƶ㣨ƥ�����������߶��㣩
            Vector3 midPoint = (startPoint + endPoint) / 2f; // ˮƽ�е�
            float apexHeight = heightFactor * (horizontalDistance / 4f); // ����߶ȣ�������켣ƥ�䣩
            Vector3 controlPoint = new Vector3(midPoint.x, startPoint.y + apexHeight, midPoint.z);

            // ��տ�����ߣ������ۻ���ֵ��
            //widthCurve.keys = new Keyframe[0];

            // ���ɱ����������ϵĵ�
            for (int i = 0; i < resolution; i++)
            {
                float tParam = (float)i / (resolution - 1); // ��һ������
                points[i] = CalculateBezierPoint(tParam, startPoint, controlPoint, endPoint);

                // ���������ߣ���ͷЧ����
                if (tParam < 0.7f)
                    currentWidth = startWidth;
                else
                    currentWidth = Mathf.Lerp(arrowStartWidth, 0, (tParam - 0.7f) / 0.3f);

                widthCurve.AddKey(tParam, currentWidth);
            }

            // Ӧ�ù켣��Ϳ������
            lineRenderer.SetPositions(points);
            lineRenderer.widthCurve = widthCurve;
        }

        // ������α����������ϵĵ�
        Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            return uu * p0 + 2 * u * t * p1 + tt * p2;
        }

        public Vector3[] GetPoints()
        {
            // �������鸱���������ⲿ�޸�
            Vector3[] copy = new Vector3[points.Length];
            Array.Copy(points, copy, points.Length);
            return copy;
        }
    }
}