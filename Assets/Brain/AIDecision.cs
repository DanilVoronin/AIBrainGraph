using UnityEngine;
using UnityEngine.Events;

namespace Brain
{
	/// <summary>
	/// Decisions are components that will be evaluated by transitions, every frame, and will return true or false. Examples include time spent in a state, distance to a target, or object detection within an area.  
	/// </summary>
	public abstract class AIDecision : MonoBehaviour
	{
		/// Decide will be performed every frame while the Brain is in a state this Decision is in. Should return true or false, which will then determine the transition's outcome.
		public abstract bool Decide();
		
		/// <summary>
		/// Вызывается при входе в состояние
		/// </summary>
		[SerializeField] private UnityEvent OnEnter;
		/// <summary>
		/// Вызывается при выходе из состояние
		/// </summary>
		[SerializeField] private UnityEvent OnExite;

		public string Label;
		protected AIBrain _brain;
        
		/// <summary>
		/// Meant to be overridden, called when the game starts
		/// </summary>
		public virtual void Initialization(AIBrain aIBrain)
		{
			_brain = aIBrain;
		}
		/// <summary>
		/// Meant to be overridden, called when the Brain enters a State this Decision is in
		/// </summary>
		public virtual void OnEnterState()
		{
			OnEnter?.Invoke();
		}
		/// <summary>
		/// Meant to be overridden, called when the Brain exits a State this Decision is in
		/// </summary>
		public virtual void OnExitState()
		{
			OnExite?.Invoke();
		}
	}
}