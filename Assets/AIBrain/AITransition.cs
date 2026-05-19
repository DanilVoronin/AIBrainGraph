using System;
using UnityEngine;

namespace Brain
{
	/// <summary>
	/// Описывает переход от состояния к состоянию
	/// </summary>
	[Serializable]
	public class AITransition
	{
		public string Label;
		public AIDecision Decision;
		public int StateTrueIndex = -1;
		public int StateFalseIndex = -1;
	}
}