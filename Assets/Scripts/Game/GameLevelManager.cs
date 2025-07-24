using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// һ����Ϸ�ڵ�һ�����ؿ��Ĺ�����
    /// </summary>
    public class GameLevelManager : Singleton<GameLevelManager>
    {
        #region �ؿ�����

        private bool _levelStarted;
        public bool levelStarted => _levelStarted;

        private bool _levelFinished;
        public bool levelFinished => _levelFinished;

        #endregion

        private float _phaseDuration = 0f; // ��ǰ�׶���ʱ��

        private List<GameLevelData> _levelDataList;

        public List<Transform> bornPointList;

        private int levelIndex;

        #region ��ҹ״̬

        public enum LevelPhase
        {
            Day,
            Night,
            End
        }

        private LevelPhase _currentPhase = LevelPhase.Day;

        private float _phaseTimer = 0f;
        private float dayDuration = 10f; // �������ʱ�䣨�룩
        private float nightDuration = 60f; // ҹ�����ʱ�䣨�룩
        private bool _timerRunning = false;

        [SerializeField] private Material skyMaterial;

        private int _totalDays = 3; // ������
        private int _currentDay = 0; // ��ǰ����
        
        public event Action<LevelPhase, float> OnPhaseTimeUpdated;

        #endregion

        public void Init(List<GameLevelData> levelDataList)
        {
            _levelDataList = levelDataList;

            EnemyManager.Instance.Init();
            LevelResManager.Instance.Init();
        }

        public void Discard()
        {
        }

        private void Update()
        {
            if (!_timerRunning) return;

            _phaseTimer -= Time.deltaTime;

            float t = Mathf.Clamp01(1f - _phaseTimer / _phaseDuration);
            OnPhaseTimeUpdated?.Invoke(_currentPhase, t);
            
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

        #region �ؿ�ѭ��

        // ��ʼ��һ��
        public void InitFirstLevel()
        {
            _currentDay = 1;
            LevelResManager.Instance.InitLevelRes(_levelDataList[0].levelResData);
            StartDayPhase();
        }

        // ��ʼ����һ�ؿ��������ﱣ����ȣ�����Ҷ���
        public void InitNextLevel()
        {
            levelIndex++;
            if (levelIndex >= _levelDataList.Count)
            {
                levelIndex = 0;
            }
            
            PlayerSaveData.Instance.AutoSave();
            LevelResManager.Instance.InitLevelRes(_levelDataList[levelIndex].levelResData);
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

        #region ��ҹѭ��

        public void StartDayPhase()
        {
            if (_currentDay > _totalDays)
            {
                // �������������
                GameManager.Instance.FinishGame();
                return;
            }
  
            _currentPhase = LevelPhase.Day;
            _phaseDuration = dayDuration;
            _phaseTimer = dayDuration;
            _timerRunning = true;

            _levelStarted = true;
            _levelFinished = false;
        }

        public void StartNightPhase()
        {
            EnemyManager.Instance.InitEnemy(_levelDataList[levelIndex].enemyDataList, bornPointList);
            
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
            
            if (_currentDay <= _totalDays)
            {
                InitNextLevel();
                StartDayPhase();
            }
            else
            {
                // �����������
                GameManager.Instance.FinishGame();
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
    }
}