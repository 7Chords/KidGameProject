using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public enum LevelPhase
    {
        Day,
        Night,
        End
    }
    
    /// <summary>
    /// 游戏关卡管理器
    /// </summary>
    public class GameLevelManager : Singleton<GameLevelManager>
    {
        #region 关卡状态

        private bool _levelStarted;
        public bool levelStarted => _levelStarted;

        private bool _levelFinished;
        public bool levelFinished => _levelFinished;

        #endregion

        private float _phaseDuration = 0f; // 当前阶段剩余时间

        private List<GameLevelData> _levelDataList;

        private int levelIndex;

        #region 昼夜状态

        private LevelPhase _currentPhase = LevelPhase.Day;

        private float _phaseTimer = 0f;
        private float dayDuration = 10f; // 白天持续时间
        private float nightDuration = 60f; // 夜晚持续时间
        private bool _timerRunning = false;

        [SerializeField] private Material skyMaterial;

        private int _totalDays = 3; // 总共的天数
        private int _currentDay = 0; // 当天是第几天

        #endregion

        public void Init(List<GameLevelData> levelDataList)
        {
            _levelDataList = levelDataList;

            EnemyManager.Instance.Init();
            LevelResManager.Instance.Init();
        }

        public void Discard()
        {
            EnemyManager.Instance.Discard();
            LevelResManager.Instance.Discard();
        }

        private void Update()
        {
            if (!_timerRunning) return;

            _phaseTimer -= Time.deltaTime;

            float t = Mathf.Clamp01(1f - _phaseTimer / _phaseDuration);
            MsgCenter.SendMsg(MsgConst.ON_PHASE_TIME_UPDATE, _currentPhase, t);
            
            float lerpValue = _currentPhase switch
            {
                LevelPhase.Day => Mathf.Lerp(1f, 0f, t),
                LevelPhase.Night => Mathf.Lerp(0f, 1f, t),
                _ => skyMaterial.GetFloat("_DayNightLerp")
            };

            skyMaterial.SetFloat("_DayNightLerp", lerpValue);

            if (_phaseTimer <= 0f)
            {
                switch (_currentPhase)
                {
                    case LevelPhase.Day:
                        StartNightPhase();
                        break;
                    case LevelPhase.Night:
                        EndNightPhase();
                        break;
                }
            }
        }

        #region 关卡循环
        
        public void InitFirstLevel()
        {
            _currentDay = 1;
            LevelResManager.Instance.InitLevelRes(_levelDataList[0].f2MMappingList, _levelDataList[0].r2MMappingList);
            StartDayPhase();
        }

        public void InitNextLevel()
        {
            levelIndex++;
            if (levelIndex >= _levelDataList.Count)
            {
                levelIndex = 0;
            }
            
            PlayerSaveData.Instance.AutoSave();
            LevelResManager.Instance.InitLevelRes(_levelDataList[levelIndex].f2MMappingList, _levelDataList[levelIndex].r2MMappingList);
        }

        public void DiscardCurrentLevel()
        {
            LevelResManager.Instance.DiscardLevelRes();
            EnemyManager.Instance.DiscardEnemy();
        }

        #endregion

        #region 昼夜循环

        public void StartDayPhase()
        {  
            _currentPhase = LevelPhase.Day;
            _phaseDuration = dayDuration;
            _phaseTimer = dayDuration;
            _timerRunning = true;

            _levelStarted = true;
            _levelFinished = false;

            MsgCenter.SendMsgAct(MsgConst.ON_LEVEL_START);
        }

        public void StartNightPhase()
        {
            EnemyManager.Instance.InitEnemy(_levelDataList[levelIndex].enemySpawnCfgList);
            
            _currentPhase = LevelPhase.Night;
            _phaseDuration = nightDuration;
            _phaseTimer = nightDuration;
            _timerRunning = true;
        }

        public void EndNightPhase()
        {
            _levelFinished = true;
            _currentPhase = LevelPhase.End;
            _timerRunning = false;
            
            _currentDay++;

            MsgCenter.SendMsgAct(MsgConst.ON_LEVEL_FINISH);
            
            if (_currentDay <= _totalDays)
            {
                DiscardCurrentLevel();
                InitNextLevel();
                StartDayPhase();
            }
            else
            {
                GameManager.Instance.GameFinish(true);
            }
        }
        
        public void SetCurrentDay(int day)
        {
            _currentDay = day;
        }
        
        public int GetCurrentDay()
        {
            return _currentDay;
        }

        #endregion

        #region 新增方法 - 用于存档系统集成

        /// <summary>
        /// 获取当前阶段剩余时间
        /// </summary>
        public float GetPhaseRemainingTime()
        {
            return _phaseTimer;
        }

        /// <summary>
        /// 获取当前阶段总时长
        /// </summary>
        public float GetPhaseTotalDuration()
        {
            return _phaseDuration;
        }

        /// <summary>
        /// 获取当前阶段进度（0-1）
        /// </summary>
        public float GetPhaseProgress()
        {
            return 1f - (_phaseTimer / _phaseDuration);
        }

        /// <summary>
        /// 暂停昼夜计时器
        /// </summary>
        public void PausePhaseTimer()
        {
            _timerRunning = false;
        }

        /// <summary>
        /// 恢复昼夜计时器
        /// </summary>
        public void ResumePhaseTimer()
        {
            _timerRunning = true;
        }

        /// <summary>
        /// 设置昼夜持续时间（用于难度调整）
        /// </summary>
        public void SetDurations(float dayTime, float nightTime)
        {
            dayDuration = dayTime;
            nightDuration = nightTime;
        }

        /// <summary>
        /// 获取当前阶段名称（用于UI显示）
        /// </summary>
        public string GetCurrentPhaseName()
        {
            return _currentPhase switch
            {
                LevelPhase.Day => "白天",
                LevelPhase.Night => "夜晚",
                LevelPhase.End => "结束",
            };
        }

        #endregion
    }
}