
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NumBalloonPop {
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
            [Header("General")]
            public int level;
            public int balloonCount;
            public float spawnRate;
            public int MinRightAns;

            [Header("Numbered Balloon Settings")]
            [Tooltip("How many balloons in this level should be 'numbered' (require multiple taps).")]
            public int numberedBalloonsCount;

            [Tooltip("Minimum required taps for numbered balloons (inclusive).")]
            public int minNumber;

            [Tooltip("Maximum required taps for numbered balloons (inclusive).")]
            public int maxNumber;

            [Header("Burst settings")]
            [Tooltip("Minimum number of balloons to spawn in a burst (>=1).")]
            public int minSpawnPerWave;

            [Tooltip("Maximum number of balloons to spawn in a burst (>= minSpawnPerWave).")]
            public int maxSpawnPerWave;

            [Tooltip("Chance (0..1) that a spawn tick becomes a burst when maxSpawnPerWave > 1.")]
            public float burstChance;
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

        [Header("Burst behavior")]
        [Tooltip("When enabled, any burst spawn (when possible) will contain at least one numbered balloon if the level still has numbered balloons left.")]
        [SerializeField] private bool MustContainNumberdBalloon = false;

        #region State
        private Coroutine _spawnRoutine;
        private int _currentLevelIndex = 0;
        private int _spawnedThisLevel = 0;
        private int _rightAnswersThisLevel = 0;
        private bool _levelActive = false;

        
        private HashSet<int> _numberedSpawnIndices = new();
        private int _numberedSpawnedThisLevel = 0;
        #endregion

        public int CurrentLevelIndex => _currentLevelIndex;

        public int CurrentLevelLabel {
            get {
                if(levelData != null && levelData.Count > 0 && _currentLevelIndex >= 0 && _currentLevelIndex < levelData.Count)
                    return levelData[_currentLevelIndex].level;
                return 0;
            }
        }

        private void OnValidate() {
            if(defaultSpawnRate <= 0f) defaultSpawnRate = 0.1f;

            if(levelData != null) {
                for(int i = 0; i < levelData.Count; i++) {
                    var temp = levelData[i];
                    temp.spawnRate = Mathf.Max(0.1f, temp.spawnRate);
                    temp.minNumber = Mathf.Max(1, temp.minNumber);
                    temp.maxNumber = Mathf.Max(temp.minNumber, temp.maxNumber);
                    temp.balloonCount = Mathf.Max(1, temp.balloonCount);

                    // Burst bounds
                    temp.minSpawnPerWave = Mathf.Max(1, temp.minSpawnPerWave);
                    temp.maxSpawnPerWave = Mathf.Max(temp.minSpawnPerWave, temp.maxSpawnPerWave);
                    temp.burstChance = Mathf.Clamp01(temp.burstChance);

                    levelData[i] = temp;
                }
            }
        }
        private void Awake() {
            gameManager = gameManager ?? BalloonPopGameManager.Instance;
        }
        private void Start() {

            if(spawnParent == null || mainCanvas == null || balloonPrefab == null || datas == null || datas.Count == 0) {
                Debug.LogError($"{nameof(BalloonSpawner)} missing references or data - disabling.");
                enabled = false;
                return;
            }

            if(levelData == null || levelData.Count == 0) {
                Debug.LogWarning($"{nameof(BalloonSpawner)}: no levelData defined — using a single default level.");
                levelData = new List<LevelData> {
                    new LevelData {
                        level = 1,
                        balloonCount = 10,
                        spawnRate = defaultSpawnRate,
                        MinRightAns = 3,
                        numberedBalloonsCount = 1,
                        minNumber = 2,
                        maxNumber = 3,
                        minSpawnPerWave = 1,
                        maxSpawnPerWave = 1,
                        burstChance = 0f
                    }
                };
            }

            if(gameManager != null) {
                gameManager.onRightPop.AddListener(OnCorrectPop);
            }

            StartGame();
        }

        private void OnDisable() {
            StopCurrentSpawn();
        }

        private void OnDestroy() {
            OnDisable();
        }

        #region Level control
        private void StartLevel(int levelIndex) {
            if(levelData == null || levelData.Count == 0) return;

            if(levelIndex < 0) levelIndex = 0;
            if(levelIndex >= levelData.Count) {
                if(loopLevels) levelIndex = 0;
                else {
                    Debug.Log($"{nameof(BalloonSpawner)}: reached final level index {levelIndex}. Stopping spawner.");
                    enabled = false;
                    return;
                }
            }

            _currentLevelIndex = levelIndex;
            _spawnedThisLevel = 0;
            _rightAnswersThisLevel = 0;
            _levelActive = true;
            _numberedSpawnedThisLevel = 0;
            _numberedSpawnIndices.Clear();

            var lvl = levelData[_currentLevelIndex];
            float rate = (lvl.spawnRate > 0f) ? lvl.spawnRate : defaultSpawnRate;

            
            int balloonCount = Mathf.Max(1, lvl.balloonCount);
            int targetNumbered = Mathf.Clamp(lvl.numberedBalloonsCount, 0, balloonCount);
            while(_numberedSpawnIndices.Count < targetNumbered) {
                int idx = UnityEngine.Random.Range(0, balloonCount);
                _numberedSpawnIndices.Add(idx);
            }

            gameManager.Invoke_LevelChange(CurrentLevelLabel);

            StopCurrentSpawn();
            _spawnRoutine = StartCoroutine(SpawnLevelRoutine(rate, lvl.balloonCount, lvl.MinRightAns));
            Debug.Log($"Starting level {lvl.level} — BaloonCount: {lvl.balloonCount} Rate:  1/{rate}s, minRight: {lvl.MinRightAns}, numberedBaloons: {targetNumbered}, burstRange: [{lvl.minSpawnPerWave},{lvl.maxSpawnPerWave}], burstChance: {lvl.burstChance}");
        }

        public int GetSpawnedThisLevel() {
            return _spawnedThisLevel;
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
            var lvl = levelData[_currentLevelIndex];

            while(_spawnedThisLevel < balloonCount && enabled) {
                bool isBurst = false;
                int spawnCount = 1;

                if(lvl.maxSpawnPerWave > 1) {
                    isBurst = UnityEngine.Random.value < lvl.burstChance;
                    spawnCount = isBurst ? UnityEngine.Random.Range(lvl.minSpawnPerWave, lvl.maxSpawnPerWave + 1) : 1;
                } else {
                    spawnCount = 1;
                }

               
                spawnCount = Mathf.Min(spawnCount, balloonCount - _spawnedThisLevel);

                
                if(isBurst && MustContainNumberdBalloon) {
                    int remainingNumbered = _numberedSpawnIndices.Count - _numberedSpawnedThisLevel;
                    if(remainingNumbered > 0) {
                        bool anyInRange = false;
                        int rangeStart = _spawnedThisLevel;
                        int rangeEnd = _spawnedThisLevel + spawnCount - 1;
                        foreach(var idx in _numberedSpawnIndices) {
                            if(idx >= rangeStart && idx <= rangeEnd) {
                                anyInRange = true;
                                break;
                            }
                        }

                        if(!anyInRange) {
                          
                            int candidate = -1;
                            foreach(var idx in _numberedSpawnIndices) {
                                if(idx > rangeEnd) { candidate = idx; break; }
                            }

                            if(candidate != -1) {
                               
                                int attempts = 0;
                                int newIdx = -1;
                                do {
                                    newIdx = UnityEngine.Random.Range(rangeStart, rangeEnd + 1);
                                    attempts++;
                                } while(_numberedSpawnIndices.Contains(newIdx) && attempts < 8);

                              
                                if(_numberedSpawnIndices.Contains(newIdx)) {
                                    for(int i = rangeStart; i <= rangeEnd; i++) {
                                        if(!_numberedSpawnIndices.Contains(i)) { newIdx = i; break; }
                                    }
                                }

                                if(newIdx >= rangeStart && newIdx <= rangeEnd) {
                                    _numberedSpawnIndices.Remove(candidate);
                                    _numberedSpawnIndices.Add(newIdx);
                                }
                            }
                        }
                    }
                }
                for(int i = 0; i < spawnCount && enabled; i++) {
                    SpawnSingleBalloonAtIndex(_spawnedThisLevel);
                    _spawnedThisLevel++;
                }

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

        #region Spawn helpers
        private void SpawnSingleBalloonAtIndex(int sequenceIndex) {
            if(!enabled) return;

            Vector2 size = spawnParent.rect.size;
            Vector2 randomPos = new Vector2(
                UnityEngine.Random.Range(-size.x / 2f, size.x / 2f),
                UnityEngine.Random.Range(-size.y / 2f, size.y / 2f)
            );

            GameObject balloonObj = Instantiate(balloonPrefab, spawnParent, true);
            if(balloonObj == null) {
                Debug.LogError("Failed to instantiate balloonPrefab.");
                return;
            }

            var rt = balloonObj.GetComponent<RectTransform>();
            if(rt == null) { Destroy(balloonObj); return; }
            rt.anchoredPosition = randomPos;

            var data = datas[UnityEngine.Random.Range(0, datas.Count)];
            var balloonComp = balloonObj.GetComponent<Balloon>();
            if(balloonComp == null) { Destroy(balloonObj); return; }

            var lvl = levelData[_currentLevelIndex];
            bool assignNumber = false;
            int assignedHits = 1;

            if(_numberedSpawnIndices.Contains(sequenceIndex)) {
                assignNumber = true;
                assignedHits = UnityEngine.Random.Range(Mathf.Max(1, lvl.minNumber), Mathf.Max(lvl.minNumber, lvl.maxNumber) + 1);
                _numberedSpawnedThisLevel++;
            }

            balloonComp.Initialize(data, mainCanvas, gameManager ?? BalloonPopGameManager.Instance, assignNumber, assignedHits);
        }
        #endregion

        #region Events
        public void OnCorrectPop() {
            if(!_levelActive) return;
            _rightAnswersThisLevel++;
            Debug.Log($"Numbered (counted) pops this level: {_rightAnswersThisLevel}");
        }

        public void StartGame() {
            StartLevel(0);
        }
        #endregion
    }
}
