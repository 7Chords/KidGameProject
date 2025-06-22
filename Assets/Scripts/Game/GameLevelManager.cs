using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// 一局游戏内的一个个关卡的管理器
    /// </summary>
    public class GameLevelManager : Singleton<GameLevelManager>
    {
        #region 关卡进度

        private bool _levelStarted;
        public bool levelStarted => _levelStarted;


        private bool _levelFinished;
        public bool levelFinished => _levelFinished;

        #endregion
        
        private float _phaseDuration = 0f; // 当前阶段总时长

        private List<GameLevelData> _levelDataList;

        public List<Transform> bornPointList;

        private int levelIndex;

        #region 昼夜状态

        private enum LevelPhase
        {
            Day,
            Night,
            End
        }

        private LevelPhase _currentPhase = LevelPhase.Day;
        private int _currentLoopScore = 0;
        public int scoreThreshold = 100;    
        
        private float _phaseTimer = 0f;
        public float dayDuration = 30f;   // 白天持续时间（秒）
        public float nightDuration = 60f; // 夜晚持续时间（秒）
        private bool _timerRunning = false;
        
        [SerializeField] private Material skyMaterial;

        #endregion
        
        public void Init(List<GameLevelData> levelDataList)
        {
            _levelDataList = levelDataList;

            EnemyManager.Instance.Init();
        }

        private void Update()
        {
            if (!_timerRunning) return;

            _phaseTimer -= Time.deltaTime;

            float t = Mathf.Clamp01(1f - (_phaseTimer / _phaseDuration));
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

        // 初始第一关
        public void InitFirstLevel()
        {
            EnemyManager.Instance.InitEnemy(_levelDataList[0].enemyDataList, bornPointList);
        }

        // 初始化下一关卡，在这里保存进度，供玩家读档
        public void InitNextLevel()
        {
            levelIndex++;
            EnemyManager.Instance.InitEnemy(_levelDataList[levelIndex].enemyDataList, bornPointList);
        }

        public void DiscardLevel()
        {
            EnemyManager.Instance.DiscardEnemy();
        }
        
        public void StartLevel()
        {
            _levelStarted = true;
        }

        public void FinishLevel()
        {
            _levelFinished = true;
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

            GameManager.Instance.ResetLoopScore();
        }
        
        public void StartNightPhase()
        {
            _currentPhase = LevelPhase.Night;
            _phaseDuration = nightDuration;
            _phaseTimer = nightDuration;
            _timerRunning = true;

            Debug.Log("敌人开始入侵，玩家需诱导敌人踩陷阱");
        }
        
        public void EndNightPhase()
        {
            _levelFinished = true;
            _currentPhase = LevelPhase.End;
            _timerRunning = false;
            
            _currentLoopScore = GameManager.Instance.GetCurrentLoopScore();

            if (_currentLoopScore >= scoreThreshold)
            {
                Debug.Log("得分足够，进入下一天");
                InitNextLevel();
                StartDayPhase(); // 自动进入下一轮
            }
            else
            {
                Debug.Log("得分不足，游戏结束");
                GameManager.Instance.FinishGame();
            }
        }

        
        #endregion
    }
}