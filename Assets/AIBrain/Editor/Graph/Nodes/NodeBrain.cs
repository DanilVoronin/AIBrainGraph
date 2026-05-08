using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Brain.Graph.Nodes
{
    /// <summary>
    /// Нод мозгов
    /// </summary>
    public class NodeBrain : AINode
    {
        public readonly AIBrain Brain;
        
        public NodeBrain(AIBrain brain) : base()
        {
            Brain = brain;
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
                Brain.FirstState = nodeState.AIState;
            }
        }

        private void StateDisconnected(Edge edge)
        {
            Brain.FirstState = null;
        }

    }
}