using System;
using System.Collections.Generic;
using UnityEngine;

namespace Brain
{
	/// <summary>
	/// A State is a combination of one or more actions, and one or more transitions. An example of a state could be "_patrolling until an enemy gets in range_".
	/// </summary>
	[Serializable]
	public class AIState 
	{
		/// the name of the state (will be used as a reference in Transitions
		public string StateName;

        /// a list of actions to perform in this state
        public List<AIAction> Actions = new List<AIAction>();
        /// a list of transitions to evaluate to exit this state
        public List<AITransition> Transitions = new List<AITransition>();

		protected AIBrain _brain;

		/// <summary>
		/// Sets this state's brain to the one specified in parameters
		/// </summary>
		/// <param name="brain"></param>
		public virtual void SetBrain(AIBrain brain)
		{
			_brain = brain;
		}
                	
		/// <summary>
		/// On enter state we pass that info to our actions and decisions
		/// </summary>
		public virtual void EnterState()
		{
			foreach (AIAction action in Actions)
			{
				action.OnEnterState();
                if (action.UpDateMode == AIAction.UpDateModes.OnlyOnce)
                {
                    action.PerformAction();
                }
            }
			foreach (AITransition transition in Transitions)
			{
				if (transition.Decision != null)
				{
					transition.Decision.OnEnterState();
				}
			}
		}

		/// <summary>
		/// On exit state we pass that info to our actions and decisions
		/// </summary>
		public virtual void ExitState()
		{
			foreach (AIAction action in Actions)
			{
				action.OnExitState();
			}
			foreach (AITransition transition in Transitions)
			{
				if (transition.Decision != null)
				{
					transition.Decision.OnExitState();
				}
			}
		}

		/// <summary>
		/// Performs this state's actions
		/// </summary>
		public virtual void PerformActions()
		{
			if (Actions.Count == 0) { return; }
			for (int i=0; i<Actions.Count; i++) 
			{
				if (Actions[i] != null)
				{
					if (Actions[i].UpDateMode == AIAction.UpDateModes.EveryTime)
					{
						Actions[i].PerformAction();
					}
				}
				else
				{
					Debug.LogError("An action in " + _brain.gameObject.name + " on state " + StateName + " is null.");
				}
			}
		}

		/// <summary>
		/// Tests this state's transitions
		/// </summary>
		public virtual void EvaluateTransitions()
		{
			if (Transitions.Count == 0) { return; }
			for (int i = 0; i < Transitions.Count; i++) 
			{
				if (Transitions[i].Decision)
				{
					if (Transitions[i].Decision.Decide())
					{
						AIState state = _brain.GetAIStateByIndex(Transitions[i].StateTrueIndex);
						if (state != null)
						{
							_brain.TransitionToState(state);
							break;
						}
					}
					else
					{
						AIState state = _brain.GetAIStateByIndex(Transitions[i].StateFalseIndex);
						if (state != null)
						{
							_brain.TransitionToState(state);
							break;
						}
					}
				}                
			}
		}
	}
}