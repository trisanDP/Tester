using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem.EnhancedTouch;
using DG.Tweening;

namespace LevelSelector {
    public class LevelUI : MonoBehaviour {

        [Header("UI")]
        public RectTransform contentParent;
        public GameObject levelPanelRoot;
        public GameObject backgroundRoot;
        
        public Button nextLevelButton;
        public Button skipLevelButton;

        [Header("Spawn Pos")]
        public float levelSpacing = 4f;
        public Vector2 startPosition = Vector2.zero;

        public LevelItem levelPrefab;

        [Header("Private")]
        List<LevelItem> spawnedLevels = new List<LevelItem>();
        Level[] currentStageLevels;
        Stage selectedStage;
        int selectedStageIndex;

        void Awake() {
            EnhancedTouchSupport.Enable();
        }

        void Start() {
            if(nextLevelButton != null) nextLevelButton.onClick.AddListener(OnNextLevelPressed);
            if(skipLevelButton != null) skipLevelButton.onClick.AddListener(OnSkipLevelPressed);
            if(levelPanelRoot != null) levelPanelRoot.SetActive(false);
        }

        public void ShowLevelsForStage(Stage stage) {
            if(stage == null) return;
            selectedStage = stage;
            selectedStageIndex = System.Array.IndexOf(LevelManager.Instance.stages, stage);
            if(selectedStageIndex < 0) selectedStageIndex = 0;
            LevelManager.Instance.SelectStage(selectedStageIndex);
            currentStageLevels = stage.levels ?? new Level[0];
            GenerateLevelsFromArray(currentStageLevels);
            if(levelPanelRoot != null) levelPanelRoot.SetActive(true);
        }

        void GenerateLevelsFromArray(Level[] levelsArray) {
            if(contentParent == null) return;
            foreach(Transform c in contentParent) Destroy(c.gameObject);
            spawnedLevels.Clear();

            var ordered = levelsArray;
            for(int i = 0; i < ordered.Length; i++) {
                var level = ordered[i];
                var instance = Instantiate(levelPrefab.gameObject, contentParent).GetComponent<LevelItem>();
                instance.name = "Level_local_" + i; 
                var rt = instance.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(startPosition.x + i * levelSpacing, startPosition.y);
                instance.SetUI(level);
                spawnedLevels.Add(instance);
            }

            int localIndex = LevelManager.Instance.CurrentStageIndex == selectedStageIndex ? LevelManager.Instance.CurrentLevelIndex : 0;
            localIndex = Mathf.Clamp(localIndex, 0, Mathf.Max(0, spawnedLevels.Count - 1));
            MoveToLevel(localIndex);
        }

        void OnNextLevelPressed() {
            StartCompleteSequence();
        }

        void OnSkipLevelPressed() {
            StartSkipSequence();
        }

        void StartCompleteSequence() {
            if(currentStageLevels == null || currentStageLevels.Length == 0) return;

            if(LevelManager.Instance.CurrentStageIndex != selectedStageIndex) {
                LevelManager.Instance.SelectStage(selectedStageIndex);
            }

            var completedLevel = LevelManager.Instance.CompleteCurrentLevel();
            if(completedLevel == null) return;

            GenerateLevelsFromArray(currentStageLevels);

            int nextIndex = LevelManager.Instance.CurrentStageIndex == selectedStageIndex ? LevelManager.Instance.CurrentLevelIndex : -1;
            if(nextIndex >= 0 && nextIndex < spawnedLevels.Count) MoveToLevel(nextIndex);
        }

        void StartSkipSequence() {
            if(currentStageLevels == null || currentStageLevels.Length == 0) return;

            if(LevelManager.Instance.CurrentStageIndex != selectedStageIndex) {
                LevelManager.Instance.SelectStage(selectedStageIndex);
            }

            var skipped = LevelManager.Instance.SkipCurrentLevel();
            if(skipped == null) return;
            
            GenerateLevelsFromArray(currentStageLevels);

            int nextIndex = LevelManager.Instance.CurrentStageIndex == selectedStageIndex ? LevelManager.Instance.CurrentLevelIndex : -1;
            if(nextIndex >= 0 && nextIndex < spawnedLevels.Count) MoveToLevel(nextIndex);
        }

        void MoveToLevel(int indexZeroBased) {
            if(contentParent == null) return;
            float targetX = -(startPosition.x + indexZeroBased * levelSpacing);
            contentParent.DOAnchorPos(new Vector2(targetX, contentParent.anchoredPosition.y), 0.45f).SetEase(Ease.OutCubic);
        }
    }
}
