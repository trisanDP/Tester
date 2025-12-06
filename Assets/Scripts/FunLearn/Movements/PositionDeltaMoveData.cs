using UnityEngine;

namespace Arcube.FunLearn.Movements
{
    [CreateAssetMenu(menuName = "Movements/PositionDeltaMoveData", fileName = "MoveData")]
    public class PositionDeltaMoveData : MoveData
    {
        public Vector3 speed;
        public Vector3 randomOffset;
    }
}