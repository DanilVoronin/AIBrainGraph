using UnityEngine;
using UnityEngine.AI;

namespace Brain.Actions.MeshAgent
{
    /// <summary>
    /// Выполняет действие движения агента к выбранной точке
    /// </summary>
    public class MeshAgentMoveToPoint : AIAction
    {
        [SerializeField] private Transform endPoint;
        private NavMeshAgent _agent;

        public override void Initialization(AIBrain aIBrain)
        {
            _agent ??= GetComponent<NavMeshAgent>();
            base.Initialization(aIBrain);
        }

        public override void OnEnterState()
        {
            _agent.SetDestination(endPoint.position);
            base.OnEnterState();
        }

        /// <summary>
        /// Устанавливает конесную точку
        /// </summary>
        /// <param name="point"></param>
        public void SetEndPoint(Transform point)
        {
            if(point == null) return;
            endPoint =  point;
        }
    }
}