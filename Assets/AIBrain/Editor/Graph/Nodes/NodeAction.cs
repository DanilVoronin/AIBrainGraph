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

        private readonly AIBrain _brain;
        
        public NodeAction(AIBrain brain):base()
        {
            _brain = brain;
            
            Port input = Port.Create<Edge>(
                Orientation.Horizontal, 
                Direction.Input, 
                Port.Capacity.Single,
                typeof(AIAction));
            
            inputContainer.Add(input);
        }

        public void InitAction<T>() where T : AIAction
        {
            if(AIAction != null) return;
                
            AIAction = _brain.gameObject.AddComponent<T>();
            
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

        protected override void OnDetachFromPanelEvent(DetachFromPanelEvent e)
        {
            base.OnDetachFromPanelEvent(e);
            if(AIAction != null) Object.DestroyImmediate(AIAction);
        }
    }
}