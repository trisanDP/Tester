using UnityEngine;

namespace Arcube.FunLearn.Movements
{
    [CreateAssetMenu(fileName = "MoveData", menuName = "Movements/PointMoveData")]
    public class PointMoveData : MoveData
    {
        public Vector3[] points;
        public Vector3 offset;
        public float speed = 1;
        public AnimationCurve offsetCurve;
    }
}