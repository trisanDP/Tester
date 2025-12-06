using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class PlanetDOTween : MonoBehaviour {
    [Header("Revolution Settings")]
    public Transform centerPoint;        
    public float revolutionSpeed = 5f;   
    public float orbitRadius = 5f;       

    
    public float rotationSpeed = 20f;   

    private Tween revolutionTween;

    private void Start() {
        StartRevolution();
        StartSelfRotation();
    }

    private void StartRevolution() {
        if(centerPoint == null) {
            Debug.LogWarning("Center Point Not Assigned");
            return;
        }

        Vector3 startPos = centerPoint.position + (transform.position - centerPoint.position).normalized * orbitRadius;
        transform.position = startPos;

        revolutionTween = DOTween.To(
            () => 0f,
            angle => {
                float rad = Mathf.Deg2Rad * angle;
                Vector3 offset = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * orbitRadius;
                transform.position = centerPoint.position + offset;
            },
            360f,
            revolutionSpeed
        )
        .SetEase(Ease.Linear)
        .SetLoops(-1)
        .SetUpdate(true);

    }

    private void StartSelfRotation() {
        transform.DORotate(new Vector3(0, 360f, 0), rotationSpeed, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .SetUpdate(true);
    }

    private void OnDisable() {
        if(revolutionTween != null && revolutionTween.IsActive())
            revolutionTween.Kill();
        transform.DOKill();
    }
}
