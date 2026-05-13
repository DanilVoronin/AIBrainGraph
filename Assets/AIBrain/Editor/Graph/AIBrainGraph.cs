using System.Collections.Generic;
using Brain.Graph.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace Brain.Graph
{
    public class AIBrainGraph : GraphView
    {
        public Vector2 worldMousePosition;
        
        private AIBrain _brain;
        
        public AIBrainGraph()
        {
            CreateGraph();
            InitGraph(Selection.activeObject);
        }

        private void CreateGraph()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContentZoomer());
            
            this.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.ClearItems();
                evt.menu.AppendAction("Добавить состояние", _ => AddState(worldMousePosition));
                evt.menu.AppendAction("Добавить переход", _ => AddTransitionNode(worldMousePosition));
            }));
            
            var miniMap = new MiniMap
            {
                anchored = false,        // Можно перемещать
                maxWidth = 250,          // Максимальная ширина
                maxHeight = 180,         // Максимальная высота
                windowed = false         // Отображать внутри GraphView (не в отдельном окне)
            };
    
            miniMap.SetPosition(new Rect(10, 30, 200, 140));
            Add(miniMap);
            
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            GridBackground gridBackground = new GridBackground();
            Insert(0, gridBackground);
            
            //Позиция мыши для корректного позицианирования нодов
            RegisterCallback<MouseDownEvent>(MouseDownEvent);

            graphViewChanged = OnGraphChange;
        }

        private GraphViewChange OnGraphChange(GraphViewChange change)
        {
            if (change.elementsToRemove != null)
            {
                foreach (var element in change.elementsToRemove)
                {
                    if (element is AINode node)
                    {
                        node.DestroyNode();
                    }
                }
            }

            return change;
        }

        private void MouseDownEvent(MouseDownEvent e)
        {
            worldMousePosition = contentViewContainer.WorldToLocal(e.mousePosition);
        }

        /// Проверка совместимости портов
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            foreach (var port in ports)
            {
                // ❌ Нельзя соединять порт с самим собой
                if (port == startPort) continue;
                
                // ❌ Нельзя соединять порты одного узла
                if (port.node == startPort.node) continue;
                
                // ❌ Направления должны быть противоположными (Input ↔ Output)
                if (port.direction == startPort.direction) continue;
                
                // ❌ Типы данных должны строго совпадать
                if (port.portType != startPort.portType) continue;
                
                compatiblePorts.Add(port);
            }
            return compatiblePorts;
        }

        #region Add

        private void AddState(Vector2 position)
        {
            var node = new NodeState(_brain);
            AddElement(node);

            node.SetPosition(new Rect(position.x - 100, position.y - 50, 200, 150));
        }
            
        public void AddActionIdle(Type action, Vector2 position)
        {
            var node = new NodeAction(_brain, action);
            node.title = action.Name;
            
            AddElement(node);
            node.SetPosition(new Rect(position.x - 100, position.y - 50, 200, 150));
        }

        private void AddTransitionNode(Vector2 position)
        {
            var node = new NodeTransition(_brain);
            AddElement(node);

            node.SetPosition(new Rect(position.x - 100, position.y - 50, 200, 150));
        }

        private void AddDecisionNode(Vector2 position)
        {
            var node = new NodeDecision(_brain);
            AddElement(node);

            node.SetPosition(new Rect(position.x - 100, position.y - 50, 200, 150));
        }
        
        #endregion
        
        #region Init
        
        private void InitGraph(object obj)
        {
            if (obj is GameObject selectedObject)
            {
                //Создание нодов
                CreateBrainNode(selectedObject);
            }
        }

        private void CreateBrainNode(GameObject selectedObject)
        {
            if (selectedObject != null)
            {
                if (selectedObject.TryGetComponent(out _brain))
                {
                    NodeBrain nodeBrain = new NodeBrain(_brain);
                    
                    AddElement(nodeBrain);
                    
                    nodeBrain.SetPosition(new Rect(250, 250, 200, 150));
                }
            }
        }
        
        #endregion Init

        public void Close()
        {
            UnregisterCallback<MouseDownEvent>(MouseDownEvent);
        }
    }
}