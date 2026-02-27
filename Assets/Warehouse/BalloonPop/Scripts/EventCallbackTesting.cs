using UnityEngine;

namespace NumBalloonPop {
    public class EventCallbackTesting : MonoBehaviour {
        public  void OnAnyPop() { Debug.Log("AnyPop"); }
        public  void OnWrongPop() { Debug.Log("Wrong Pop"); }

        public  void OnNumberedPop() { Debug.Log("Numbered/Right Pop"); }

        public void OnLevelChange(int level) { Debug.Log("Level Change to Level : " + level); }
    }
}
