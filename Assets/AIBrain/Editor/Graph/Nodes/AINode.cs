using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Brain.Graph.Nodes
{
    /// <summary>
    /// Базовый нод
    /// </summary>
    public class AINode : Node
    {
        public AINode() : base()
        {
            capabilities |= Capabilities.Selectable |
                            Capabilities.Movable |
                            Capabilities.Deletable;
            
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanelEvent);
        }

        protected virtual void OnDetachFromPanelEvent(DetachFromPanelEvent e)
        {
            
        }
    }
}