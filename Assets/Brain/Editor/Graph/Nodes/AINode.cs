using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

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
            capabilities &= ~Capabilities.Deletable;
            capabilities |= Capabilities.Selectable |
                            Capabilities.Movable;
            
            this.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.ClearItems();
                evt.menu.AppendAction("Удалить", _ => RemoveSelf());
            }));
        }
        
        private void RemoveSelf()
        {
            var graphView = GetFirstAncestorOfType<GraphView>();
            if (graphView != null)
            {
                // Найдём все рёбра, связанные с этим нодом, и удалим их
                var connectedEdges = graphView.edges.ToList()
                    .Where(edge => edge.input.node == this || edge.output.node == this);

                foreach (var edge in connectedEdges)
                {
                    graphView.RemoveElement(edge);
                }

                // Теперь удаляем сам нод
                graphView.RemoveElement(this);
            }
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