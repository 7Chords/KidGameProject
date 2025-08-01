﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KidGame.UI.Editor
{
    /// <summary>
    /// 1.创建UI框架结构:
    /// 在场景中直接创建UI框架：通过菜单项"Assets/Create/UI/UI Frame in Scene"
    /// 创建UI框架预制体：通过菜单项"Assets/Create/UI/UI Frame Prefab"
    /// 2.自动构建的UI框架组件
    /// </summary>
    public static class UIFrameworkTools
    {
        [MenuItem("Assets/Create/UI/UI Frame in Scene", priority = 2)]
        public static void CreateUIFrameInScene() {
            CreateUIFrame();
        }
        
        [MenuItem("Assets/Create/UI/UI Frame Prefab", priority = 1)]
        public static void CreateUIFramePrefab() {
            var frame = CreateUIFrame();
            
            string prefabPath = GetCurrentPath();
            prefabPath = EditorUtility.SaveFilePanel("UI Frame Prefab", prefabPath,"UIFrame", "prefab");

            if (prefabPath.StartsWith(Application.dataPath)) {
                prefabPath =  "Assets" + prefabPath.Substring(Application.dataPath.Length);
            }

            if (!string.IsNullOrEmpty(prefabPath)) {
                CreateNewPrefab(frame, prefabPath);    
            }

            Object.DestroyImmediate(frame);
        }

        private static GameObject CreateUIFrame() {
            var uiLayer = LayerMask.NameToLayer("UI");
            var root = new GameObject("UIFrame");
            var camera = new GameObject("UICamera");

            var cam = camera.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Depth;
            cam.cullingMask = LayerMask.GetMask("UI");
            cam.orthographic = true;
            cam.farClipPlane = 25;

            root.AddComponent<UIFrame>();
            var canvas = root.AddComponent<Canvas>();
            root.layer = uiLayer;

            //  ScreenSpaceCamera 允许使用3D模型、粒子效果
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;

            cam.transform.SetParent(root.transform, false);
            cam.transform.localPosition = new Vector3(0f, 0f, -1500f);

            var screenScaler = root.AddComponent<CanvasScaler>();
            screenScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            screenScaler.referenceResolution = new Vector2(1920, 1080);

            root.AddComponent<GraphicRaycaster>();

            var eventSystem = new GameObject("EventSystem");
            eventSystem.transform.SetParent(root.transform, false);
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            // 创建层
            var panelLayerGO = CreateRect("PanelLayer", root, uiLayer);
            var panelLayer = panelLayerGO.AddComponent<PanelUILayer>();

            var windowLayerGO = CreateRect("WindowLayer", root, uiLayer);
            var windowLayer = windowLayerGO.AddComponent<WindowUILayer>();

            var prioPanelLayer = CreateRect("PriorityPanelLayer", root, uiLayer);

            var windowParaLayerGO = CreateRect("PriorityWindowLayer", root, uiLayer);
            var windowParaLayer = windowParaLayerGO.AddComponent<WindowParaLayer>();
            // 通过反射来设置参数
            SetPrivateField(windowLayer, windowParaLayer, "priorityParaLayer");

            var darkenGO = CreateRect("DarkenBG", windowParaLayer.gameObject, uiLayer);
            var darkenImage = darkenGO.AddComponent<Image>();
            darkenImage.color = new Color(0f, 0f, 0f, 0.75f);
            // 通过反射设置蒙黑
            SetPrivateField(windowParaLayer, darkenGO, "darkenBgObject");
            darkenGO.SetActive(false);

            var tutorialPanelLayer = CreateRect("TutorialPanelLayer", root, uiLayer);

            // 在面板层上装配参数
            var prioList = new List<PanelPriorityLayerListEntry>();
            prioList.Add(new PanelPriorityLayerListEntry(PanelPriority.None, panelLayer.transform));
            prioList.Add(new PanelPriorityLayerListEntry(PanelPriority.Prioritary, prioPanelLayer.transform));
            prioList.Add(new PanelPriorityLayerListEntry(PanelPriority.Tutorial, tutorialPanelLayer.transform));
            var panelPrios = new PanelPriorityLayerList(prioList);

            SetPrivateField(panelLayer, panelPrios, "priorityLayers");

            return root;
        }

        public static string GetCurrentPath() {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "") {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "") {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            return path;
        }
        
        private static void SetPrivateField(object target, object element, string fieldName) {
            var prop = target.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance);
            prop.SetValue(target, element);
        }

        private static GameObject CreateRect(string name, GameObject parentGO, int layer) {
            var parent = parentGO.GetComponent<RectTransform>();
            var newRect = new GameObject(name, typeof(RectTransform));
            newRect.layer = layer;
            var rt = newRect.GetComponent<RectTransform>();

            rt.anchoredPosition = parent.position;
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.transform.SetParent(parent, false);
            rt.sizeDelta = Vector3.zero;

            return newRect;
        }
        
        private static void CreateNewPrefab(GameObject obj, string localPath) {
            PrefabUtility.SaveAsPrefabAsset(obj, localPath);
        }
    }
}