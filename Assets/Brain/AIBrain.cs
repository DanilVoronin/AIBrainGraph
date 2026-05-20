using System.Collections.Generic;
using UnityEngine;

namespace Brain
{
	[AddComponentMenu("Tools/AI/AIBrain")]
	public class AIBrain : MonoBehaviour
	{
		/// <summary>
		/// Первое состояние
		/// </summary>
		public AIState FirstState
		{
			get => GetAIStateByIndex(indexFirstState);
			set => indexFirstState = States.IndexOf(value);
		}
		
		[Header("Debug")]
		public List<AIState> States;
		public AIState CurrentState { get; protected set; }
		public float TimeInThisState;

		public Vector3 LastKnownTargetPosition = Vector3.zero;

		[Header("State")]
		public bool BrainActive = true;

		protected AIDecision[] _decisions;
		protected AIAction[] _actions;
		protected ITarget _target;

		[SerializeField]
		private int indexFirstState = -1;
		
		public ITarget Target 
		{ 
			get
			{
				if (_target != null)
				{
					if (!_target.GameObject.activeSelf)
					{ 
						_target = null;
					}
				}
				return _target;
			}
			private set
			{
				if (_target != value)
				{
					_target = value;
				}
			}
		}

		/// <summary>
		/// Возвращает состояние по индексу
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public AIState GetAIStateByIndex(int index)
		{
			if(States.Count == 0) return  null;
			if (index < 0 || index >= States.Count) return null;
			return States[index];
		}
		/// <summary>
		/// Устанавливает новую цель
		/// </summary>
		/// <param name="target"></param>
		public void SetTarget(ITarget target)
		{
			if (target != Target)
			{
                Target = target;
            }
		}
		/// <summary>
		/// Получает и возвращает компоненты действий
		/// </summary>
		/// <returns></returns>
		public virtual AIAction[] GetAttachedActions()
		{
			AIAction[] actions = this.gameObject.GetComponentsInChildren<AIAction>();
			return actions;
		}
		/// <summary>
		/// Получает и возвращает решения которые оцениваются для перехода в другое состояние
		/// </summary>
		/// <returns></returns>
		public virtual AIDecision[] GetAttachedDecisions()
		{
			AIDecision[] decisions = this.gameObject.GetComponentsInChildren<AIDecision>();
			return decisions;
		}
		/// <summary>
		/// Инициализация мозгов
		/// </summary>
		public virtual void InitBrain()
		{
            foreach (AIState state in States)
				state.SetBrain(this);

			_decisions = GetAttachedDecisions();
			_actions = GetAttachedActions();

            ResetBrain();
        }

        public void Awake()
        {
			InitBrain();
		}

        /// <summary>
        /// Метод обновления мозгов. Тактируется характером
        /// Вызывается в  Update
        /// </summary>
        public virtual void Update()
		{
			if (!BrainActive || (CurrentState == null) || (Time.timeScale == 0f))
			{
				return;
			}

			CurrentState.PerformActions();
            
			if (!BrainActive)
			{
				return;
			}
            
			CurrentState?.EvaluateTransitions();
            
			TimeInThisState += Time.deltaTime;

			StoreLastKnownPosition();
		}
        /// <summary>
        /// Переходы в указанное состояние, запуск событий выхода и входа в состояния
        /// </summary>
        /// <param name="newStateName"></param>
        public virtual void TransitionToState(AIState newState)
		{
			if (CurrentState == null)
			{
				CurrentState = FindState(newState);
				if (CurrentState != null)
				{
					CurrentState.EnterState();
				}
				return;
			}
			if (newState != CurrentState)
			{
				AIState state = FindState(newState);

				if (state != null)
				{
					CurrentState.ExitState();
					OnExitState();

					CurrentState = state;
					CurrentState.EnterState();
				}
			}
		}
        /// <summary>
        /// При выходе из состояния мы сбрасываем счетчик времени
        /// </summary>
        protected virtual void OnExitState()
		{
			TimeInThisState = 0f;
		}
        /// <summary>
        /// Инициализирует все решения
        /// </summary>
        protected virtual void InitializeDecisions()
		{
			if (_decisions == null)
			{
				_decisions = GetAttachedDecisions();
			}
			foreach(AIDecision decision in _decisions)
			{
				decision.Initialization(this);
			}
		}
        /// <summary>
        /// Инициализирует все действий
        /// </summary>
        protected virtual void InitializeActions()
		{
			if (_actions == null)
			{
				_actions = GetAttachedActions();
			}
			foreach(AIAction action in _actions)
			{
				action.Initialization(this);
			}
		}
        /// <summary>
        /// Возвращает состояние на основе указанного имени состояния.
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        protected AIState FindState(AIState findState)
		{
			foreach (AIState state in States)
			{
				if (state == findState)
				{
					return state;
				}
			}
			return null;
		}
		/// <summary>
		/// Stores the last known position of the target
		/// </summary>
		protected virtual void StoreLastKnownPosition()
		{
			if (Target != null)
			{
				LastKnownTargetPosition = Target.GameObject.transform.position;
			}
		}
        /// <summary>
        /// Перезагружает мозг, заставляя его войти в первое состояние
        /// </summary>
        public virtual void ResetBrain()
		{
			InitializeDecisions();
			InitializeActions();
			BrainActive = true;
			this.enabled = true;

			if (CurrentState != null)
			{
				CurrentState.ExitState();
				OnExitState();
			}
            
			if (FirstState != null)
			{
				CurrentState = FirstState;
				CurrentState?.EnterState();
			}  
		}

		/// <summary>
		/// Сбрасывает цель
		/// </summary>
		public virtual void ResetTarget()
		{
			_target = null;
		}

        private void OnDrawGizmosSelected()
        {
			//Линия от персонажа до цели
			if (Target != null)
			{
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, Target.GameObject.transform.position);
            }
        }
    }
}