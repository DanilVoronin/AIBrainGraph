using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Brain.Graph.Nodes
{
    /// <summary>
    /// Нод мозгов
    /// </summary>
    public class NodeBrain : AINode
    {
        public Port AIStatePort { get; private set; }
        
        public NodeBrain(AIBrain brain) : base(brain)
        {
            title = "Brain";
            
            AIStatePort = new AIPort<AIState>(
                Direction.Output,
                Port.Capacity.Single,
                StateConnected, 
                StateDisconnected)
            {
                portName = "FirstState"
            };

            outputContainer.Add(AIStatePort);
            
            SetColor(AIGraphSettings.ColorNodeBrain);
        }

        private void StateConnected(Edge edge)
        {
            if(edge.input.node is NodeState nodeState)
            {
                _brain.FirstState = nodeState.AIState;
            }
        }

        private void StateDisconnected(Edge edge)
        {
            _brain.FirstState = null;
        }

    }
}