using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Brain.Graph.Nodes
{
    /// <summary>
    /// Нод действия
    /// </summary>
    public class NodeAction : AINodeComponent
    {
        public Port AIStatePort  { get; private set; }
        public AIAction AIAction { get; private set; }
        
        public event Action<string> OnChangeLableAction;

        public NodeAction()
        {
            AIStatePort = Port.Create<Edge>(
                Orientation.Horizontal, 
                Direction.Input, 
                Port.Capacity.Single,
                typeof(AIAction));
            
            inputContainer.Add(AIStatePort);
            
            SetColor(AIGraphSettings.ColorNodeAction);
        }
        
        public override void Setup(AIBrain brain, Type component)
        {
            _brain = brain;
            AIAction = (AIAction)_brain.gameObject.AddComponent(component);
            CreateGUI();
        }
        
        public override void Setup(AIBrain brain, Component component)
        {
            _brain = brain;
            AIAction = (AIAction)component;
            CreateGUI();
        }

        private void CreateGUI()
        {
            var textField = new TextField
            {
                value = AIAction.Label,
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    width = 100,
                }
            };

            textField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                AIAction.Label = evt.newValue;
                OnChangeLableAction?.Invoke(evt.newValue);
            });
            
            titleContainer.Add(textField);
            
            _serializedObject = new SerializedObject(AIAction);
            
            Initialize(AIAction);
        }

        public override void DestroyNode()
        {
            if(AIAction != null)
            {
                Undo.DestroyObjectImmediate(AIAction);
            }
        }
    }
}