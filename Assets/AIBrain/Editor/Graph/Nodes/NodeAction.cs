using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Brain.Graph.Nodes
{
    /// <summary>
    /// Нод действия
    /// </summary>
    public class NodeAction : AINode
    {
        public AIAction AIAction { get; private set; }
        
        public event Action<string> OnChangeLableAction;

        public NodeAction(AIBrain brain, Type action):base(brain)
        {
            Port input = Port.Create<Edge>(
                Orientation.Horizontal, 
                Direction.Input, 
                Port.Capacity.Single,
                typeof(AIAction));
            
            inputContainer.Add(input);

            InitAction(action);
        }

        private void InitAction(Type action)
        {
            if(AIAction != null) return;
                
            //TODO
            AIAction = (AIAction)_brain.gameObject.AddComponent(action);
            
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
        }

        public override void DestroyNode()
        {
            if(AIAction != null) Object.DestroyImmediate(AIAction);
        }
    }
}