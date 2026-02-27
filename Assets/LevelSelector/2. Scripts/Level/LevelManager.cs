using NaughtyAttributes;
using System.Linq;
using UnityEngine;

namespace LevelSelector {
    public class LevelManager : SingletonMonoBehaviour<LevelManager> {
        public Stage[] stages;
        public Level[] Levels { get; private set; }
        int currentStageIndex;
        int currentLevelIndex;
        SaveData saveData;
        public int HighestStageUnlocked { get; private set; }

        protected override void Awake() {
            base.Awake();
            Levels = stages.Where(s => s != null && s.levels != null).SelectMany(s => s.levels).ToArray();
            saveData = SaveSystem.LoadGame();
            ApplySaveToLevels();
        }

        void ApplySaveToLevels() {  //Saved Data Load Garni 
            if(saveData == null) saveData = new SaveData { highestStageUnlocked = 1, currentStageIndex = 0, currentLevelIndex = 0, levels = new LevelSave[0] };

            //resets current level data
            if(stages != null) {
                for(int si = 0; si < stages.Length; si++) {
                    var stage = stages[si];
                    if(stage == null || stage.levels == null) continue;
                    for(int li = 0; li < stage.levels.Length; li++) {
                        var lvl = stage.levels[li];
                        if(lvl == null) continue;
                        lvl.Score = 0;
                        lvl.StarCount = 0;
                        lvl.State = LevelState.Locked;
                    }
                }
            }


            //apply saved data
            if(saveData.levels != null && saveData.levels.Length > 0) {
                for(int i = 0; i < saveData.levels.Length; i++) {
                    var entry = saveData.levels[i];
                    if(entry.stageIndex < 0 || entry.stageIndex >= (stages?.Length ?? 0)) continue;
                    var stage = stages[entry.stageIndex];
                    if(stage == null || stage.levels == null) continue;
                    var lvl = System.Array.Find(stage.levels, l => l != null && l.levelID == entry.levelID);
                    if(lvl == null) continue;
                    lvl.StarCount = entry.starCount;
                    lvl.Score = entry.score;
                    lvl.State = (LevelState)entry.state;
                }
            }


            //set current indices and highest unlocked stage
            HighestStageUnlocked = Mathf.Clamp(saveData.highestStageUnlocked <= 0 ? 1 : saveData.highestStageUnlocked, 1, Mathf.Max(1, stages?.Length ?? 1));
            currentStageIndex = Mathf.Clamp(saveData.currentStageIndex, 0, Mathf.Max(0, (stages?.Length ?? 1) - 1));
            var stageForCurrent = (stages != null && stages.Length > 0) ? stages[currentStageIndex] : null;
            int maxLevelIdx = (stageForCurrent != null && stageForCurrent.levels != null) ? stageForCurrent.levels.Length - 1 : 0;
            currentLevelIndex = Mathf.Clamp(saveData.currentLevelIndex, 0, Mathf.Max(0, maxLevelIdx));


            //lock/unlock levels based on highest unlocked stage
            for(int si = 0; si < stages.Length; si++) {
                var stage = stages[si];
                if(stage == null || stage.levels == null) continue;
                bool unlockedStage = (si + 1) <= HighestStageUnlocked;
                if(!unlockedStage) {
                    for(int li = 0; li < stage.levels.Length; li++) {
                        var lvl = stage.levels[li];
                        if(lvl != null) lvl.State = LevelState.Locked;
                    }
                } else {
                    for(int li = 0; li < stage.levels.Length; li++) {
                        var lvl = stage.levels[li];
                        if(lvl == null) continue;
                        if(lvl.State == LevelState.Locked) lvl.State = LevelState.Unlocked;
                    }
                }
            }

            //set current level to active
            if(stages != null && stages.Length > 0) {
                var curStage = stages[Mathf.Clamp(currentStageIndex, 0, stages.Length - 1)];
                if(curStage != null && curStage.levels != null && curStage.levels.Length > 0) {
                    var curLvl = curStage.levels[Mathf.Clamp(currentLevelIndex, 0, curStage.levels.Length - 1)];
                    if(curLvl != null) curLvl.State = LevelState.Active;
                }
            }

            //if no saved data, initialize first level as active
            bool hasAnySavedEntry = saveData.levels != null && saveData.levels.Length > 0;
            if(!hasAnySavedEntry) {
                HighestStageUnlocked = 1;
                currentStageIndex = 0;
                currentLevelIndex = 0;
                if(stages != null && stages.Length > 0) {
                    var first = stages[0];
                    if(first != null && first.levels != null && first.levels.Length > 0) {
                        for(int li = 0; li < first.levels.Length; li++) {
                            var lvl = first.levels[li];
                            if(lvl == null) continue;
                            lvl.State = (li == 0) ? LevelState.Active : LevelState.Locked;
                            lvl.Score = 0;
                            lvl.StarCount = 0;
                        }
                    }
                }
            }
        }

        public int CurrentStageIndex => currentStageIndex;
        public int CurrentLevelIndex => currentLevelIndex;

        public void SelectStage(int stageIndex) {
            if(stages == null || stageIndex < 0 || stageIndex >= stages.Length) return;
            currentStageIndex = Mathf.Clamp(stageIndex, 0, stages.Length - 1);
            var s = stages[currentStageIndex];
            if(s == null || s.levels == null || s.levels.Length == 0) {
                currentLevelIndex = 0;
            } else {
                int found = System.Array.FindIndex(s.levels, l => l != null && l.State == LevelState.Active);
                currentLevelIndex = found >= 0 ? found : 0;
            }
            ApplySelectionStates();
        }


        void ApplySelectionStates() {
            
            for(int si = 0; si < stages.Length; si++) {
                var stage = stages[si];
                if(stage == null || stage.levels == null) continue;

                bool isStageFullyCleared = (si + 1) < HighestStageUnlocked;
                bool isFutureStage = (si + 1) > HighestStageUnlocked;

                for(int li = 0; li < stage.levels.Length; li++) {
                    var lvl = stage.levels[li];
                    if(lvl == null) continue;

                    if(isFutureStage) {
                        lvl.State = LevelState.Locked;
                    } else {
                        if(si == currentStageIndex) {
                            if(li == currentLevelIndex) {
                                lvl.State = LevelState.Active;
                            } else if(li < currentLevelIndex) {
                                lvl.State = lvl.StarCount > 0 ? LevelState.Completed : LevelState.Unlocked;
                            } else {
                                if(isStageFullyCleared || lvl.StarCount > 0 || lvl.Score > 0) {
                                    lvl.State = lvl.StarCount > 0 ? LevelState.Completed : LevelState.Unlocked;
                                } else {
                                    lvl.State = LevelState.Locked;
                                }
                            }
                        } else {
                            if(lvl.State == LevelState.Locked) lvl.State = LevelState.Unlocked;
                        }
                    }
                }
            }
        }

        public void SaveProgress() {
            int highestStage = HighestStageUnlocked;
            for(int si = 0; si < stages.Length; si++) {
                var stage = stages[si];
                if(stage == null || stage.levels == null) continue;
                bool anyUnlocked = false;
                for(int li = 0; li < stage.levels.Length; li++) {
                    var lvl = stage.levels[li];
                    if(lvl != null && lvl.State != LevelState.Locked) { anyUnlocked = true; break; }
                }
                if(anyUnlocked) highestStage = Mathf.Max(highestStage, si + 1);
            }
            HighestStageUnlocked = highestStage;
            saveData.highestStageUnlocked = HighestStageUnlocked;
            saveData.currentStageIndex = currentStageIndex;
            saveData.currentLevelIndex = currentLevelIndex;
            SaveSystem.SaveGame(HighestStageUnlocked, currentStageIndex, currentLevelIndex, stages);
        }

        public int CalculateStars(Level lvl) {
            if(lvl == null) return 0;
            int max = Mathf.Max(1, lvl.maxScore);
            float score = Mathf.Clamp(lvl.Score, 0f, max);
            float pct = (score / max) * 100f;
            //Debug.Log("Calculating stars for level " + lvl.levelID + " with score " + lvl.Score + " / " + max + " (" + pct + "%)");
            if(pct < 33f) return 1;
            if(pct < 66f) return 2;
            return 3;
        }

        public Level CompleteCurrentLevel() {
            if(stages == null || stages.Length == 0) return null;

            var stage = stages[Mathf.Clamp(currentStageIndex, 0, stages.Length - 1)];
            if(stage == null || stage.levels == null || stage.levels.Length == 0) return null;

            var lvl = stage.levels[Mathf.Clamp(currentLevelIndex, 0, stage.levels.Length - 1)];
            if(lvl == null) return null;

            //Logic to Save Score and Stars
            int max = Mathf.Max(1, lvl.maxScore);
            int newScore = Random.Range(max / 2, max + 1);
            if(newScore > lvl.Score) lvl.Score = newScore;

            int earnedStars = CalculateStars(lvl);
            if(earnedStars > lvl.StarCount) lvl.StarCount = earnedStars;

            lvl.State = LevelState.Completed;

            //next stage unlock garni
            if(currentLevelIndex >= stage.levels.Length - 1) {
                if(currentStageIndex + 1 < stages.Length) {
                    HighestStageUnlocked = Mathf.Max(HighestStageUnlocked, currentStageIndex + 2); 
                }
            } else {
                currentLevelIndex++;
            }

            ApplySelectionStates();
            SaveProgress();
            return lvl;
        }

        public Level SkipCurrentLevel() {
            if(stages == null || stages.Length == 0) return null;
            var stage = stages[Mathf.Clamp(currentStageIndex, 0, stages.Length - 1)];
            if(stage == null || stage.levels == null || stage.levels.Length == 0) return null;
            var lvl = stage.levels[Mathf.Clamp(currentLevelIndex, 0, stage.levels.Length - 1)];
            if(lvl == null) return null;
            lvl.State = LevelState.Completed;
            lvl.StarCount = 0;
            currentLevelIndex++;
            if(currentLevelIndex >= stage.levels.Length) {
                currentStageIndex++;
                currentLevelIndex = 0;
                if(currentStageIndex >= stages.Length) currentStageIndex = stages.Length - 1;
                HighestStageUnlocked = Mathf.Max(HighestStageUnlocked, currentStageIndex + 1);
            }
            ApplySelectionStates();
            SaveProgress();
            return lvl;
        }

        [Button]
        void DeleteData() {
            SaveSystem.DeleteSave();
        }

        public Level GetLevelInStage(int stageIndex, int localIndex) {
            if(stages == null || stageIndex < 0 || stageIndex >= stages.Length) return null;
            var stage = stages[stageIndex];
            if(stage == null || stage.levels == null) return null;
            if(localIndex < 0 || localIndex >= stage.levels.Length) return null;
            return stage.levels[localIndex];
        }
    }
}
