using NaughtyAttributes;
using UnityEngine;

namespace Arcube.FunLearn.Movements
{
    [CreateAssetMenu(menuName = "Movements/PhysicsMoveData", fileName = "MoveData")]
    public class PhysicsMoveData : MoveData
    {

    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class PhysicsMover2D : ObjectMover
    {
        [Expandable] 
        [SerializeField] PhysicsMoveData moveData;
        public override void Move()
        {

        }

        public override void Move(Vector3 target)
        {

        }
    }
}