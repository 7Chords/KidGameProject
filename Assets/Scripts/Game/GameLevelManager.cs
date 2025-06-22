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
        public float dayDuration = 30f;   // �������ʱ�䣨�룩
        public float nightDuration = 60f; // ҹ�����ʱ�䣨�룩
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

        #region �ؿ�ѭ��

        // ��ʼ��һ��
        public void InitFirstLevel()
        {
            EnemyManager.Instance.InitEnemy(_levelDataList[0].enemyDataList, bornPointList);
        }

        // ��ʼ����һ�ؿ��������ﱣ����ȣ�����Ҷ���
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
        
        #region ��ҹѭ��

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

            Debug.Log("���˿�ʼ���֣�������յ����˲�����");
        }
        
        public void EndNightPhase()
        {
            _levelFinished = true;
            _currentPhase = LevelPhase.End;
            _timerRunning = false;
            
            _currentLoopScore = GameManager.Instance.GetCurrentLoopScore();

            if (_currentLoopScore >= scoreThreshold)
            {
                Debug.Log("�÷��㹻��������һ��");
                InitNextLevel();
                StartDayPhase(); // �Զ�������һ��
            }
            else
            {
                Debug.Log("�÷ֲ��㣬��Ϸ����");
                GameManager.Instance.FinishGame();
            }
        }

        
        #endregion
    }
}