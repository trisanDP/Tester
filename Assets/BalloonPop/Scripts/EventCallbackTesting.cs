using UnityEngine;

namespace NaniBabu.BalloonPop {
    public class EventCallbackTesting : MonoBehaviour {
        public static void OnPop() {
            Debug.Log("AnyPop");
        }

        public static void OnRightPop() {
            Debug.Log("Right Pop");

        }

        public static void OnWrongPop() {
            Debug.Log("Wrong Pop");

        }

        public static void OnChangeColor() {
            Debug.Log("Changed Main Color");
        }
    }
}