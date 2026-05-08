using System.Collections.Generic;
using Brain.Actions;
using Brain.Graph.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Brain.Graph
{
    public class AIBrainGraph : GraphView
    {
        public Vector2 worldMousePosition;
        
        private AIBrain _brain;
        
        public AIBrainGraph()
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
                evt.menu.AppendAction("Добавить действие", _ => AddActionIdle(worldMousePosition));
                evt.menu.AppendAction("Добавить переход", _ => AddTransitionNode(worldMousePosition));
            }));
            
            var miniMap = new MiniMap
            {
                anchored = false,        // Можно перемещать
                maxWidth = 250,          // Максимальная ширина
                maxHeight = 180,         // Максимальная высота
                windowed = false         // Отображать внутри GraphView (не в отдельном окне)
            };
    
            // Позиционирование в правом нижнем углу
            miniMap.SetPosition(new Rect(10, 30, 200, 140));
            Add(miniMap);
            
            // Зум
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // Сетка
            GridBackground gridBackground = new GridBackground();
            Insert(0, gridBackground);
            
            //Позиция мыши для корректного позицианирования нодов
            RegisterCallback<MouseDownEvent>(e =>
            {
                worldMousePosition = contentViewContainer.WorldToLocal(e.mousePosition);
            });

            InitGraph(Selection.activeObject);
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

        private void CreateNode(Vector2 center)
        {
            //var node = new Node<AIBrain>("Brain");
            //node.SetPosition(new Rect(center.x - 100, center.y - 50, 200, 150));
            //
            //AddElement(node);
        }

        #region Add

        private void AddState(Vector2 position)
        {
            var node = new NodeState(_brain);
            AddElement(node);

            node.SetPosition(new Rect(position.x - 100, position.y - 50, 200, 150));
        }
            
        private void AddActionIdle(Vector2 position)
        {
            var node = new NodeAction(_brain);
            node.InitAction<ActionIdle>();
            node.title = "ActionIdle";
            
            AddElement(node);
            node.SetPosition(new Rect(position.x - 100, position.y - 50, 200, 150));
        }

        private void AddTransitionNode(Vector2 position)
        {
            var node = new NodeTransition();
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
                }
            }
        }
        
        #endregion Init
    }
}