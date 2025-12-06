using UnityEngine;
using UnityEngine.Events;

namespace Arcube.FunLearn.Movements
{
    public enum MoveState
    {
        Idle,
        MoveStart,
        Moving,
        MoveStop,
    }

    public abstract class ObjectMover : MonoBehaviour
    {
        public bool useRectTransform = false;
        public bool lookAtTarget = false;

        public UnityEvent onMoveStart;
        public UnityEvent onMoveStop;

        public void MoveStart() => onMoveStart?.Invoke();
        public void MoveStop() => onMoveStop?.Invoke();

        /// <summary>
        /// frame by frame movent
        /// </summary>
        public abstract void Move();
        
        /// <summary>
        /// continuous movent
        /// </summary>
        /// <param name="target"></param>
        public abstract void Move(Vector3 target);
    }
}