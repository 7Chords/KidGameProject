using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class GameLevelInfo
    {
        #region 关卡状态

        private bool _levelStarted;
        public bool levelStarted => _levelStarted;

        private bool _levelFinished;
        public bool levelFinished => _levelFinished;

        #endregion

        private float _phaseDuration = 0f; // 当前阶段剩余时间

        private List<GameLevelData> _levelDataList = new List<GameLevelData>();

        private int levelIndex;

        #region 昼夜状态

        private LevelPhase _currentPhase = LevelPhase.Day;

        private float _phaseTimer = 0f;
        private float dayDuration = 10f; // 白天持续时间
        private float nightDuration = 60f; // 夜晚持续时间
        private bool _timerRunning = false;


        private int _totalDays = 3; // 总共的天数
        private int _currentDay = 0; // 当天是第几天

        #endregion

        #region 属性字段
        public bool LevelStarted
        {
            get => _levelStarted;
            set => _levelStarted = value;
        }

        public bool LevelFinished
        {
            get => _levelFinished;
            set => _levelFinished = value;
        }

        public float PhaseDuration
        {
            get => _phaseDuration;
            set => _phaseDuration = value;
        }

        public List<GameLevelData> LevelDataList
        {
            get => _levelDataList;
            set => _levelDataList = value;
        }

        public int LevelIndex
        {
            get => levelIndex;
            set => levelIndex = value;
        }
        public LevelPhase CurrentPhase
        {
            get => _currentPhase;
            set => _currentPhase = value;
        }

        public float PhaseTimer
        {
            get => _phaseTimer;
            set => _phaseTimer = value;
        }

        public float DayDuration
        {
            get => dayDuration;
            set => dayDuration = value;
        }

        public float NightDuration
        {
            get => nightDuration;
            set => nightDuration = value;
        }

        public bool TimerRunning
        {
            get => _timerRunning;
            set => _timerRunning = value;
        }

        public int TotalDays
        {
            get => _totalDays;
            set => _totalDays = value;
        }

        public int CurrentDay
        {
            get => _currentDay;
            set => _currentDay = value;
        }
        #endregion

    }

}
