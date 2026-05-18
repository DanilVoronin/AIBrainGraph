using System;
using Codice.Client.Common;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Brain.Graph.Nodes
{
    /// <summary>
    /// Нод переходов
    /// </summary>
    public class NodeTransition : AINode
    {
        public AITransition AITransition { get; private set; }  
        
        public event Action<string> OnChangeLableTrasition;
        
        public Port AITransitionPort { get; private set; }
        
        public NodeTransition(AIBrain brain, AITransition aiTransition) : base(brain)
        {
            title = "Transition";
            
            AITransition = aiTransition;
            
            var textField = new TextField
            {
                value = AITransition.Label,
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    width = 100,
                }
            };

            textField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                AITransition.Label = evt.newValue;
                OnChangeLableTrasition?.Invoke(evt.newValue);
            });
            
            titleContainer.Add(textField);

            AITransitionPort = Port.Create<Edge>(
                Orientation.Horizontal,
                Direction.Input,
                Port.Capacity.Single,
                typeof(AITransition));
            
            inputContainer.Add(AITransitionPort);
            
            outputContainer.Add(new AIPort<AIDecision>(
                Direction.Output, 
                Port.Capacity.Single,
                edge =>
                {
                    AITransition.Decision = ((NodeDecision)edge.input.node)?.AIDecision;
                },
                edge =>
                {
                    AITransition.Decision = null;
                })
            {
                portName = "Decision"
            });
            
            outputContainer.Add(new AIPort<AIState>(
                Direction.Output, 
                Port.Capacity.Single,
                ConnectionTrueState,
                DisconnectedTrueState)
            {
                portName = "TrueState"
            });
            
            outputContainer.Add(new AIPort<AIState>(
                Direction.Output, 
                Port.Capacity.Single,
                ConnectionFalseState,
                DisconnectedFalseState)
            {
                portName = "FalseState"
            });
        }
        
        private void ConnectionTrueState(Edge edge)
        {
            NodeState nodeState  = EdgeValid(edge, Direction.Input);
            if (nodeState != null)
            {
                AITransition.TrueState = nodeState.AIState.StateName;
            }
        }
        private void DisconnectedTrueState(Edge edge)
        {
            AITransition.TrueState = string.Empty;
        }

        private void ConnectionFalseState(Edge edge)
        {
            NodeState nodeState  = EdgeValid(edge, Direction.Input);
            if (nodeState != null)
            {
                AITransition.FalseState = nodeState.AIState.StateName;
            }
        }
        private void DisconnectedFalseState(Edge edge)
        {
            AITransition.FalseState = string.Empty;
        }


        private NodeState EdgeValid(Edge edge, Direction direction)
        {
            if(edge == null) return null;

            switch (direction)
            {
                case Direction.Input:
                    if (edge.input.node is NodeState stateInput) return stateInput;
                    break;
                case Direction.Output:
                    if (edge.output.node is NodeState stateOutput) return stateOutput;
                    break;
            }

            return null;
        }
    }
}