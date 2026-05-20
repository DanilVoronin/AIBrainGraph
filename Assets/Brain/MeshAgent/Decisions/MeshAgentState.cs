using Brain.MeshAgent.Models;
using UnityEngine;
using UnityEngine.AI;

namespace Brain.MeshAgent.Decisions
{
    /// <summary>
    /// Проверяет стату агента
    /// TODO для демо сцены, потом оптимизировать
    /// </summary>
    public class MeshAgentState : AIDecision
    {
        [Header("Условие")]
        [SerializeField, Tooltip("Целевое состояние")] 
        private AgentState targetState;
        
        [Header("Настройки")]
        [Tooltip("Минимальное расстояние, считающееся 'прибытием'")]
        [Range(0.1f, 1f)]
        [SerializeField]
        private float arrivalThreshold = 0.2f;
        
        [SerializeField] 
        private AgentState currentState;
        
        private NavMeshAgent _agent;
        
        public override void Initialization(AIBrain aIBrain)
        {
            _agent ??= GetComponent<NavMeshAgent>();
            base.Initialization(aIBrain);
        }

        public override void OnEnterState()
        {
            SetState(AgentState.Idle);
            base.OnEnterState();
        }

        public override bool Decide()
        {
            UpdateAgentState();
            return targetState == currentState;
        }
        
        private void UpdateAgentState()
        {
            // ✅ Агент считается прибывшим, если:
            // - У него есть просчитанный путь
            // - Он почти у цели (remainingDistance < stoppingDistance)
            // - Он не в процессе пересчёта пути
            bool hasReached = _agent.hasPath && 
                              _agent.remainingDistance <= _agent.stoppingDistance + arrivalThreshold &&
                              !_agent.pathPending;

            if (hasReached)
            {
                // 🎯 Дошли!
                if (currentState != AgentState.Arrived)
                {
                    SetState(AgentState.Arrived);
                    _agent.isStopped = true;
                }
            }
            else if (_agent.hasPath && !_agent.pathPending)
            {
                // 🚶 Всё ещё в пути
                if (currentState != AgentState.Moving)
                {
                    SetState(AgentState.Moving);
                    _agent.isStopped = false;
                }
            }
        }
        private void SetState(AgentState newState)
        {
            if (currentState == newState) return;
            currentState = newState;
        }
    }
}