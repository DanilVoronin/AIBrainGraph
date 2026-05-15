using UnityEditor.Experimental.GraphView;

namespace Brain.Graph.Nodes
{
    /// <summary>
    /// Базовый нод
    /// </summary>
    public class AINode : Node
    {
        protected AIBrain _brain;

        protected AINode()
        {
            Init();
        }

        protected AINode(AIBrain brain) : base()
        {
            _brain = brain;
            Init();
        }

        private void Init()
        {
            capabilities |= Capabilities.Selectable |
                            Capabilities.Movable |
                            Capabilities.Deletable;
        }

        public virtual void DestroyNode()
        {
            
        }
    }
}