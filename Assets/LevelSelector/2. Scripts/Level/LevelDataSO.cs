using UnityEngine;

namespace LevelSelector {
    [CreateAssetMenu(fileName = "LevelData",menuName = "Game/LevelData")]
    public class LevelDataSO : ScriptableObject {

        [Header("State")]
        public Sprite completedImg;
        public Sprite lockedImg;
        public Sprite activeImg;
        
        [Header("Difficulty")]
        public Sprite hardDiffImg;
        public Sprite midDiffImg;
        public Sprite easyDiffImg;


        


    }
}
