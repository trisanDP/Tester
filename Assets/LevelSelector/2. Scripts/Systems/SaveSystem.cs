using System.Collections.Generic;
using UnityEngine;

namespace LevelSelector {
    public static class SaveSystem {
        const string SAVE_KEY = "saveData";

        public static void SaveGame(int highestStageUnlocked, int currentStageIndex, int currentLevelIndex, Stage[] stages) {
            SaveData data = new SaveData();
            data.highestStageUnlocked = highestStageUnlocked;
            data.currentStageIndex = currentStageIndex;
            data.currentLevelIndex = currentLevelIndex;
            var list = new List<LevelSave>();
            if(stages != null) {
                for(int s = 0; s < stages.Length; s++) {
                    var stage = stages[s];
                    if(stage == null || stage.levels == null) continue;
                    for(int i = 0; i < stage.levels.Length; i++) {
                        var lvl = stage.levels[i];
                        if(lvl == null) continue;
                        var entry = new LevelSave();
                        entry.stageIndex = s;
                        entry.levelID = lvl.levelID;
                        entry.starCount = lvl.StarCount;
                        entry.score = lvl.Score;
                        entry.state = (int)lvl.State;
                        list.Add(entry);
                    }
                }
            }
            data.levels = list.ToArray();
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        public static SaveData LoadGame() {
            if(PlayerPrefs.HasKey(SAVE_KEY)) {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                return data;
            } else {
                return new SaveData { highestStageUnlocked = 1, currentStageIndex = 0, currentLevelIndex = 0, levels = new LevelSave[0] };
            }
        }

        public static void DeleteSave() {
            if(PlayerPrefs.HasKey(SAVE_KEY)) PlayerPrefs.DeleteKey(SAVE_KEY);
        }
    }

    [System.Serializable]
    public class SaveData {
        public int highestStageUnlocked;
        public int currentStageIndex;
        public int currentLevelIndex;
        public LevelSave[] levels;
    }

    [System.Serializable]
    public class LevelSave {
        public int stageIndex;
        public int levelID;
        public int starCount;
        public int score;
        public int state;
    }
}
