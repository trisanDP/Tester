using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaniBabu.BalloonPop {
    [RequireComponent(typeof(RectTransform))]
    public class BalloonSpawner : MonoBehaviour {
        #region Types
        [System.Serializable]
        public struct BalloonData {
            public Sprite sprite;
            public BalloonColor color;
        }

        [System.Serializable]
        public struct LevelData {
            public int level;          // level index/label
            public int balloonCount;   // number of balloons to spawn for this level
            public float spawnRate;    // seconds between spawns (float)
            public int MinRightAns;    // minimum correct pops required to advance (if required)
        }
        #endregion

        #region Inspector
        public RectTransform spawnParent;
        public RectTransform mainCanvas;
        public GameObject balloonPrefab;
        [SerializeField] private List<BalloonData> datas;
        [SerializeField] private float defaultSpawnRate = 1f;
        [SerializeField] private BalloonPopGameManager gameManager;
        #endregion

        [Header("Level")]
        [SerializeField] public List<LevelData> levelData;
        [SerializeField] private bool requireRightAnswersToAdvance = true;
        [SerializeField] private bool loopLevels = false;

        #region State
        private Coroutine _spawnRoutine;
        private int _currentLevelIndex = 0;
        private int _spawnedThisLevel = 0;
        private int _rightAnswersThisLevel = 0;
        private bool _levelActive = false;

        private void OnValidate() {
            if(defaultSpawnRate <= 0f) defaultSpawnRate = 0.1f;

            if(levelData != null) {
                for(int i = 0; i < levelData.Count; i++) {
                    var temp = levelData[i];
                    temp.spawnRate = Mathf.Max(0.1f, temp.spawnRate);
                    levelData[i] = temp;
                }
            }
        }


        private void Start() {
            gameManager = gameManager ?? BalloonPopGameManager.Instance;

            if(spawnParent == null || mainCanvas == null || balloonPrefab == null || datas == null || datas.Count == 0) {
                Debug.LogError($"BalloonSpawner:  missing references or data - disabling.");
                enabled = false;
                return;
            }

            if(levelData == null || levelData.Count == 0) {
                Debug.LogWarning($"BalloonSpawner: no levelData defined — using a single default level.");
                levelData = new List<LevelData> {
                    new LevelData { level = 1, balloonCount = 10, spawnRate = defaultSpawnRate, MinRightAns = 3 }
                };
            }

            if(gameManager != null) {
                gameManager.onCorrectPop.AddListener(OnCorrectPop);
            }

        }

        public void StartGame() {
            StartLevel(0);
        }

        private void OnDisable() {
            StopCurrentSpawn();
            if(gameManager != null) gameManager.onCorrectPop.RemoveListener(OnCorrectPop);
        }

        private void OnDestroy() {
            OnDisable();
        }
        #endregion

        #region Level control
        private void StartLevel(int levelIndex) {
            if(levelData == null || levelData.Count == 0) return;

            if(levelIndex < 0) levelIndex = 0;
            if(levelIndex >= levelData.Count) {
                if(loopLevels) levelIndex = levelIndex % levelData.Count;
                else {
                    Debug.Log($"BalloonSpawner: reached final level index {levelIndex}. Stopping spawner.");
                    enabled = false;
                    return;
                }
            }

            _currentLevelIndex = levelIndex;
            _spawnedThisLevel = 0;
            _rightAnswersThisLevel = 0;
            _levelActive = true;

            var lvl = levelData[_currentLevelIndex];
            float rate = (lvl.spawnRate > 0f) ? lvl.spawnRate : defaultSpawnRate;

            StopCurrentSpawn();
            _spawnRoutine = StartCoroutine(SpawnLevelRoutine(rate, lvl.balloonCount, lvl.MinRightAns));
            Debug.Log($"Starting level {_currentLevelIndex} (label {lvl.level}) — spawn {lvl.balloonCount} @ {rate}s, minRight {lvl.MinRightAns}");
        }

        private void NextLevel() {
            StopCurrentSpawn();
            _levelActive = false;
            int next = _currentLevelIndex + 1;
            StartLevel(next);

        }

        private void StopCurrentSpawn() {
            if(_spawnRoutine != null) {
                StopCoroutine(_spawnRoutine);
                _spawnRoutine = null;
            }
        }

        public void RestartCurrentLevel() => StartLevel(_currentLevelIndex);
        public void StartLevelByIndex(int index) => StartLevel(index);
        #endregion

        #region Spawn coroutine
        private IEnumerator SpawnLevelRoutine(float spawnLoopRate, int balloonCount, int minRightAns) {
            while(_spawnedThisLevel < balloonCount && enabled) {
                SpawnBalloon();
                _spawnedThisLevel++;
                yield return new WaitForSeconds(spawnLoopRate);
            }

            _spawnRoutine = null;

            if(!requireRightAnswersToAdvance) {
                NextLevel();
                yield break;
            }

            if(minRightAns <= 0) {
                NextLevel();
                yield break;
            }

            while(_rightAnswersThisLevel < minRightAns && enabled) {
                yield return null;
            }

            NextLevel();
        }
        #endregion

        #region Spawn
        void SpawnBalloon() {
            if(!enabled) return;

            Vector2 size = spawnParent.rect.size;
            Vector2 randomPos = new Vector2(
                Random.Range(-size.x / 2f, size.x / 2f),
                Random.Range(-size.y / 2f, size.y / 2f)
            );

            GameObject balloonObj = Instantiate(balloonPrefab, spawnParent, false);
            if(balloonObj == null) {
                Debug.LogError("Failed to instantiate balloonPrefab.");
                return;
            }

            var rt = balloonObj.GetComponent<RectTransform>();
            if(rt == null) { Destroy(balloonObj); return; }
            rt.anchoredPosition = randomPos;

            var data = datas[Random.Range(0, datas.Count)];
            var balloonComp = balloonObj.GetComponent<Balloon>();
            if(balloonComp == null) { Destroy(balloonObj); return; }

            balloonComp.Initialize(data, mainCanvas, gameManager ?? BalloonPopGameManager.Instance);
        }
        #endregion

        #region Events
        private void OnCorrectPop(BalloonColor color) {
            if(!_levelActive) return;
            _rightAnswersThisLevel++;
            Debug.Log($"Correct pops this level: {_rightAnswersThisLevel}");
        }
        #endregion
    }
}
