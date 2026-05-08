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
        
        public NodeTransition() : base()
        {
            title = "Transition";
            
            AITransition = new AITransition();
            
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
            
            inputContainer.Add(Port.Create<Edge>(
                Orientation.Horizontal, 
                Direction.Input, 
                Port.Capacity.Single,
                typeof(AITransition)));
            
            outputContainer.Add(new AIPort<AIState>(
                Direction.Output, 
                Port.Capacity.Single,
                ConnectionTrueState,
                DisconnectedTrueState)
            {
                title = "TrueState"
            });
            
            outputContainer.Add(new AIPort<AIState>(
                Direction.Output, 
                Port.Capacity.Single,
                ConnectionFalseState,
                DisconnectedFalseState)
            {
                title = "FalseState"
            });
        }

        private void ConnectionTrueState(Edge edge) { }
        private void DisconnectedTrueState(Edge edge) { }
        
        private void ConnectionFalseState(Edge edge) { }
        private void DisconnectedFalseState(Edge edge) { }
    }
}