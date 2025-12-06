using UnityEngine;

namespace Arcube.FunLearn.Movements
{
    [CreateAssetMenu(fileName = "MoveData", menuName = "Movements/PathMoveData")]
    public class PathMoveData : MoveData
    {
        public bool loop = false;
        public float speed = 1;
        
        public Vector3 offset;
        public AnimationCurve offsetCurve;
    }
}