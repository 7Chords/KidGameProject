﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.UI
{
    /// <summary>
    /// 这个layer层控制所有的窗口.
    /// 有显示记录和队列的，并且一次只显示一个
    /// </summary>
    public class WindowUILayer : UILayer<IWindowController>
    {
        [SerializeField] private WindowParaLayer priorityParaLayer = null;

        public IWindowController CurrentWindow { get; private set; }

        private Queue<WindowHistoryEntry> windowQueue;
        private Stack<WindowHistoryEntry> windowHistory;

        public event Action RequestScreenBlock;
        public event Action RequestScreenUnblock;

        private bool IsScreenTransitionInProgress
        {
            get { return screensTransitioning.Count != 0; }
        }

        private HashSet<IScreenController> screensTransitioning;

        public override void Initialize()
        {
            base.Initialize();
            registeredScreens = new Dictionary<string, IWindowController>();
            windowQueue = new Queue<WindowHistoryEntry>();
            windowHistory = new Stack<WindowHistoryEntry>();
            screensTransitioning = new HashSet<IScreenController>();
        }

        protected override void ProcessScreenRegister(string screenId, IWindowController controller)
        {
            base.ProcessScreenRegister(screenId, controller);
            controller.InTransitionFinished += OnInAnimationFinished;
            controller.OutTransitionFinished += OnOutAnimationFinished;
            controller.CloseRequest += OnCloseRequestedByWindow;
        }

        protected override void ProcessScreenUnregister(string screenId, IWindowController controller)
        {
            base.ProcessScreenUnregister(screenId, controller);
            controller.InTransitionFinished -= OnInAnimationFinished;
            controller.OutTransitionFinished -= OnOutAnimationFinished;
            controller.CloseRequest -= OnCloseRequestedByWindow;
        }

        public override void ShowScreen(IWindowController screen)
        {
            ShowScreen<IWindowProperties>(screen, null);
        }

        public override void ShowScreen<TProp>(IWindowController screen, TProp properties)
        {
            IWindowProperties windowProp = properties as IWindowProperties;

            if (ShouldEnqueue(screen, windowProp))
            {
                EnqueueWindow(screen, properties);
            }
            else
            {
                DoShow(screen, windowProp);
            }
        }

        public override void HideScreen(IWindowController screen)
        {
            if (screen == CurrentWindow)
            {
                windowHistory.Pop();
                AddTransition(screen);
                screen.Hide();

                CurrentWindow = null;

                if (windowQueue.Count > 0)
                {
                    ShowNextInQueue();
                }
                else if (windowHistory.Count > 0)
                {
                    ShowPreviousInHistory();
                }
            }
            else
            {
                Debug.LogError(
                    string.Format(
                        "[WindowUILayer] Hide requested on WindowId {0} but that's not the currently open one ({1})! Ignoring request.",
                        screen.ScreenId, CurrentWindow != null ? CurrentWindow.ScreenId : "current is null"));
            }
        }

        public override void HideAll(bool shouldAnimateWhenHiding = true)
        {
            base.HideAll(shouldAnimateWhenHiding);
            CurrentWindow = null;
            priorityParaLayer.RefreshDarken();
            windowHistory.Clear();
        }

        public override void ReparentScreen(IScreenController controller, Transform screenTransform)
        {
            IWindowController window = controller as IWindowController;

            if (window == null)
            {
                Debug.LogError("[WindowUILayer] Screen " + screenTransform.name + " is not a Window!");
            }
            else
            {
                if (window.IsPopup)
                {
                    priorityParaLayer.AddScreen(screenTransform);
                    return;
                }
            }

            base.ReparentScreen(controller, screenTransform);
        }

        private void EnqueueWindow<TProp>(IWindowController screen, TProp properties) where TProp : IScreenProperties
        {
            windowQueue.Enqueue(new WindowHistoryEntry(screen, (IWindowProperties)properties));
        }

        private bool ShouldEnqueue(IWindowController controller, IWindowProperties windowProp)
        {
            if (CurrentWindow == null && windowQueue.Count == 0)
            {
                return false;
            }

            //不使用controller的属性？
            if (windowProp != null && windowProp.SuppressPrefabProperties)
            {
                return windowProp.WindowQueuePriority != WindowPriority.ForceForeground;
            }

            if (controller.WindowPriority != WindowPriority.ForceForeground)
            {
                return true;
            }

            return false;
        }

        private void ShowPreviousInHistory()
        {
            if (windowHistory.Count > 0)
            {
                WindowHistoryEntry window = windowHistory.Pop();
                DoShow(window);
            }
        }

        private void ShowNextInQueue()
        {
            if (windowQueue.Count > 0)
            {
                WindowHistoryEntry window = windowQueue.Dequeue();
                DoShow(window);
            }
        }

        private void DoShow(IWindowController screen, IWindowProperties properties)
        {
            DoShow(new WindowHistoryEntry(screen, properties));
        }

        private void DoShow(WindowHistoryEntry windowEntry)
        {
            if (CurrentWindow == windowEntry.Screen)
            {
                Debug.LogWarning(
                    string.Format(
                        "[WindowUILayer] The requested WindowId ({0}) is already open! This will add a duplicate to the " +
                        "history and might cause inconsistent behaviour. It is recommended that if you need to open the same" +
                        "screen multiple times (eg: when implementing a warning message pop-up), it closes itself upon the player input" +
                        "that triggers the continuation of the flow."
                        , CurrentWindow.ScreenId));
            }
            //如果之前显示的界面属于失去焦点就隐藏，并且现在要显示的界面不是弹窗，就把之前的隐藏
            else if (CurrentWindow != null
                     && CurrentWindow.HideOnForegroundLost
                     && !windowEntry.Screen.IsPopup)
            {
                CurrentWindow.Hide();
            }

            windowHistory.Push(windowEntry);
            AddTransition(windowEntry.Screen);

            if (windowEntry.Screen.IsPopup)
            {
                priorityParaLayer.DarkenBG();
            }

            windowEntry.Show();

            CurrentWindow = windowEntry.Screen;
        }

        private void OnInAnimationFinished(IScreenController screen)
        {
            RemoveTransition(screen);
        }

        private void OnOutAnimationFinished(IScreenController screen)
        {
            RemoveTransition(screen);
            var window = screen as IWindowController;
            if (window.IsPopup)
            {
                priorityParaLayer.RefreshDarken();
            }
        }

        private void OnCloseRequestedByWindow(IScreenController screen)
        {
            HideScreen(screen as IWindowController);
        }

        private void AddTransition(IScreenController screen)
        {
            screensTransitioning.Add(screen);
            if (RequestScreenBlock != null)
            {
                RequestScreenBlock();
            }
        }

        private void RemoveTransition(IScreenController screen)
        {
            screensTransitioning.Remove(screen);
            if (!IsScreenTransitionInProgress)
            {
                if (RequestScreenUnblock != null)
                {
                    RequestScreenUnblock();
                }
            }
        }
    }
}