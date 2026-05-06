using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Brain.Graph
{
    public class AIBrainGraph : GraphView
    {
        public Vector2 worldMousePosition;
        
        private GameObject _currentSelectedObject;
        private AIBrain _currentComponent;
        
        public AIBrainGraph()
        {
            Selection.selectionChanged += OnSelectionChanged;
            
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContentZoomer());
            
            this.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.ClearItems();
                evt.menu.AppendAction("Добавить", _ => CreateNode(worldMousePosition));
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
            
            // Set up zoom capabilities
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            // Add a grid background
            GridBackground gridBackground = new GridBackground();
            Insert(0, gridBackground);
            
            RegisterCallback<MouseDownEvent>(e =>
            {
                worldMousePosition = contentViewContainer.WorldToLocal(e.mousePosition);
            });
        }
        
        /// Проверка совместимости портов (ОБЯЗАТЕЛЬНО)
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
        
        private void OnSelectionChanged()
        {
            object obj = Selection.activeObject;
            
            if (obj is GameObject selectedGo && selectedGo != _currentSelectedObject)
            {
                _currentSelectedObject = selectedGo;
                RefreshGraph();
            }
            else if (!(obj is GameObject))
            {
                _currentSelectedObject = null;
                RefreshGraph();
            }
        }
        
        private void RefreshGraph()
        {
            // Очистка текущих узлов
            graphElements.ForEach(element => RemoveElement(element));

            if (_currentSelectedObject != null)
            {
                _currentComponent = _currentSelectedObject.GetComponent<AIBrain>();

                if (_currentComponent != null)
                {
                    // Создание узла для компонента
                    var node = new Node<AIBrain>("Brain", _currentComponent);
                    AddElement(node);
                }
            }
        }
        
        public void RequestRepaint()
        {
            RefreshGraph();
        }
    }
}