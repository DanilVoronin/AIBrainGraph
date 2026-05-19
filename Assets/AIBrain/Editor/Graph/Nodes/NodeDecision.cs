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
    public class NodeDecision : AINodeComponent
    {
        public AIDecision AIDecision { get; private set; }
        
        public Port AITransitionPort { get; private set; }
        
        public event Action<string> OnChangeLableDecision;

        public NodeDecision()
        {
            AITransitionPort = Port.Create<Edge>(
                Orientation.Horizontal, 
                Direction.Input, 
                Port.Capacity.Single,
                typeof(AIDecision));
            
            inputContainer.Add(AITransitionPort);
            
            SetColor(AIGraphSettings.ColorNodeDecision);
        }


        public override void DestroyNode()
        {
            if(AIDecision != null) Object.DestroyImmediate(AIDecision);
        }

        public override void Setup(AIBrain brain, Type component)
        {
            _brain = brain;
            AIDecision = (AIDecision)_brain.gameObject.AddComponent(component);
            CreateGUI();
        }

        public override void Setup(AIBrain brain, Component component)
        {
            _brain = brain;
            AIDecision = (AIDecision)component;
            CreateGUI();
        }

        private void CreateGUI()
        {
            var textField = new TextField
            {
                value = AIDecision.Label,
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    width = 100,
                }
            };

            textField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                AIDecision.Label = evt.newValue;
                OnChangeLableDecision?.Invoke(evt.newValue);
            });
            
            titleContainer.Add(textField);
            
            Initialize(AIDecision);
        }
    }
}