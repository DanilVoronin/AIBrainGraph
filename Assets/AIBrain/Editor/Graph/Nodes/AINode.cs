using UnityEditor.Experimental.GraphView;

namespace Brain.Graph.Nodes
{
    /// <summary>
    /// Базовый нод
    /// </summary>
    public class AINode : Node
    {
        protected readonly AIBrain _brain;
        
        public AINode(AIBrain brain) : base()
        {
            _brain = brain;
            
            capabilities |= Capabilities.Selectable |
                            Capabilities.Movable |
                            Capabilities.Deletable;
        }

        public virtual void DestroyNode()
        {
            
        }
    }
}