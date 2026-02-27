using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelSelector {
    public class StageUI : MonoBehaviour {

        [Header("UI")]
        public GameObject stagePanel;
        public RectTransform contentParent;
        public GameObject levelPanel;
        public Button backButton;

        [Header("Spawn Control")]
        public Vector2 startPosition = Vector2.zero;
        public float stageSpacing = 200f;

        [Header("References")]
        public LevelUI levelUI;
        public StageItem stagePrefab;


        List<StageItem> spawned = new List<StageItem>();

        void Start() {
            if(backButton != null) backButton.onClick.AddListener(OnBackPressed);
            ShowStagePanel();
        }

        public void ShowStagePanel() {
            if(stagePanel != null) stagePanel.SetActive(true);
            if(levelPanel != null) levelPanel.SetActive(false);
            GenerateStages();
        }

        void GenerateStages() {
            if(contentParent == null || LevelManager.Instance == null) return;
            foreach(Transform t in contentParent) Destroy(t.gameObject);
            spawned.Clear();
            var stages = LevelManager.Instance.stages;
            int unlockedStage = LevelManager.Instance.HighestStageUnlocked;
            for(int i = 0; i < stages.Length; i++) {
                var s = stages[i];
                var inst = Instantiate(stagePrefab.gameObject, contentParent).GetComponent<StageItem>();
                inst.Init(s, i, OnStageSelected, (i + 1) <= unlockedStage);
                inst.name = "Stage_" + (i + 1);
                var rt = inst.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(startPosition.x + i * stageSpacing, startPosition.y);
                spawned.Add(inst);
            }
        }

        void OnStageSelected(Stage stage) {
            if(stagePanel != null) stagePanel.SetActive(false);
            if(levelPanel != null) levelPanel.SetActive(true);
            if(levelUI != null) levelUI.ShowLevelsForStage(stage);
        }

        void OnBackPressed() {
            ShowStagePanel();
        }
    }
}
