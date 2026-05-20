using UnityEditor.Experimental.GraphView;
using UnityEngine;

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

        /// <summary>
        /// Устанавливает цвет элемента
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            style.backgroundColor =  color;
            elementTypeColor =  color;
        }
    }
}