using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Brain.Graph.Nodes
{
    /// <summary>
    /// Нод мозгов
    /// </summary>
    public class NodeBrain : AINode
    {
        public NodeBrain(AIBrain brain) : base(brain)
        {
            title = "Brain";
            
            var output = new AIPort<AIState>(
                Direction.Output,
                Port.Capacity.Single,
                StateConnected, 
                StateDisconnected)
            {
                portName = "FirstState"
            };

            outputContainer.Add(output);
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