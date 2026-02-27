using UnityEngine;

namespace LevelSelector {
    public enum LevelDifficulty {
        Easy,
        Medium,
        Hard
    }
    public enum LevelState {
        Locked,
        Unlocked,
        Active,
        Completed
    }
    [CreateAssetMenu(fileName = "Level", menuName = "Game/Level")]
    public class Level : ScriptableObject {
        public int levelID;
        public LevelDifficulty difficulty;
        public static int count;
        public int maxScore;
        public int Score { get; set; }
        public int StarCount { get; set; }
        public LevelState State { get; set; }

        private void Reset() {
            levelID = count++;
            int i = UnityEngine.Random.Range(0, 3);
            difficulty = (LevelDifficulty)i;
            maxScore = 60 + i * UnityEngine.Random.Range(10, 20);
            Score = 0;
            StarCount = 0;
            State = LevelState.Locked;
        }
    }
}
