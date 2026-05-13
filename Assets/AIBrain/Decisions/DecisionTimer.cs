using UnityEngine;

namespace Brain.Decisions
{
    /// <summary>
    /// Простой таймер, осуществляет переход по истечению времени Interval
    /// </summary>
    public class DecisionTimer : AIDecision
    {
        [field: SerializeField] public float Interval { get; private set; }
        
        private float _startTime;
        
        public override bool Decide()
        {
            return Time.time - _startTime >= Interval;
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            _startTime = Time.time;
        }
    }
}