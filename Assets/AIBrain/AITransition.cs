using System;

namespace Brain
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class AITransition
	{
		public string Label;
		public AIDecision Decision;
		public string TrueState;
		public string FalseState;
	}
}