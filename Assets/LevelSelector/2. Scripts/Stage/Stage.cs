using UnityEngine;

namespace LevelSelector {

    [CreateAssetMenu(fileName = "Stage", menuName = "Game/Stage")]
    public class Stage : ScriptableObject{

        public Level[] levels;  
        public Sprite icon;
        public int StageId;
    }
}
