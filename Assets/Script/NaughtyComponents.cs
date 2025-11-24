using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class NaughtyComponent : MonoBehaviour {

    [SerializeField] private float speed = 1f;

    private void Start() {
        
    }

    private void Update() {
        //GetComponent<Rigidbody>().AddForce(Vector3.up, ForceMode.Impulse);
        MethodOne();
    }

    [Button]
    private void MethodOne() {
        transform.Translate(Vector3.up * speed);
    }

    [Button("Down")]
    private void MethodTwo() {
        transform.Translate(Vector3.down * speed);
    }
}