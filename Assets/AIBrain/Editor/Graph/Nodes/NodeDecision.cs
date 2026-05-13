using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Brain.Graph.Nodes
{
    /// <summary>
    /// Нод решения (условия для перехода)
    /// </summary>
    public class NodeDecision : AINode
    {
        public AIAction AIAction { get; private set; }
        
        public event Action<string> OnChangeLableAction;

        public NodeDecision(AIBrain brain):base(brain)
        {
            Port input = Port.Create<Edge>(
                Orientation.Horizontal, 
                Direction.Input, 
                Port.Capacity.Single,
                typeof(AIDecision));
            
            inputContainer.Add(input);
        }


        public override void DestroyNode()
        {
            //if(AIAction != null) Object.DestroyImmediate(AIAction);
        }
    }
}