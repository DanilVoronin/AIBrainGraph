using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Brain.Graph.Nodes;

namespace Brain.Graph.GUI
{
    /// <summary>
    /// Список узлов определенного типа
    /// </summary>
    public class AIListNodes<TBase, TNode> : VisualElement where TNode : AINodeComponent, new()
    {
        private readonly List<Type> _nodeTypes = new List<Type>();
        private readonly ListView _nodeListView;
        private readonly VisualElement _root;
        private readonly AIBrainGraph _aIBrainGraph;
        
        private Type _draggingNodeType;
        private VisualElement _nodePreview;
        
        public AIListNodes(string name,AIBrainGraph aIBrainGraph, VisualElement root)
        {
            _root = root;
            _aIBrainGraph = aIBrainGraph;
            
            _nodeTypes = TypeCache.GetTypesDerivedFrom<TBase>()
                .Where(t => !t.IsAbstract)
                .ToList();
            
            // Заголовок
            var header = new Label(name);
            header.AddToClassList("unity-label");
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.paddingBottom = 5;
            Add(header);
            
            _nodeListView = new ListView(
                _nodeTypes, // Заполним позже
                30,
                MakeItem,
                BindItem
            );
            
            _nodeListView.selectionType = SelectionType.Single;
            _nodeListView.showBorder = true;
            Add(_nodeListView);            
        }
        
        private VisualElement MakeItem()
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.paddingLeft = 8;
            container.style.height = 28;

            container.AddManipulator(new Dragger());
            
            var icon = new VisualElement();
            icon.style.width = 16;
            icon.style.height = 16;
            icon.style.marginRight = 8;
            icon.AddToClassList("node-icon");
            container.Add(icon);

            var label = new Label();
            label.AddToClassList("node-label");
            container.Add(label);

            return container;
        }

        private void BindItem(VisualElement element, int index)
        {
            var nodeTypes = _nodeListView.itemsSource as List<Type>;
            var nodeType = nodeTypes[index];
        
            var label = element.Q<Label>();
            label.text = ObjectNames.NicifyVariableName(nodeType.Name);
        
            // Добавляем цветовую индикацию по типу
            var icon = element.Q<VisualElement>(className: "node-icon");
            icon.style.backgroundColor = GetColorForNodeType(nodeType);

            SetupDragHandlers(label, nodeType);
        }
        
        #region DragAndDrop

        private void SetupDragHandlers(VisualElement element, Type nodeType)
        {
            element.RegisterCallback<MouseDownEvent>(evt => 
            {
                if (evt.button != 0) return;
            
                _draggingNodeType = nodeType;
            
                CreateDragPreview(element, evt.mousePosition);
            
                _root.RegisterCallback<MouseMoveEvent>(OnGlobalMouseMove);
                _root.RegisterCallback<MouseUpEvent>(OnGlobalMouseUp);
            
                evt.StopPropagation();
                evt.PreventDefault();
            });
        }
        
        private void CreateDragPreview(VisualElement sourceElement, Vector2 position)
        {
            // Создаем preview-элемент
            _nodePreview = new VisualElement
            {
                name = "node-preview",
                style = {
                    position = Position.Absolute,
                    left = position.x,
                    top = position.y,
                    width = 160,
                    height = 40,
                    backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f, 0.9f)),
                    borderRightWidth = 4,
                    cursor = new StyleCursor(StyleKeyword.Auto)
                }
            };
        
            // Добавляем иконку
            var icon = new VisualElement
            {
                style = {
                    position = Position.Absolute,
                    left = 5,
                    top = 10,
                    width = 20,
                    height = 20,
                    backgroundColor = GetColorForNodeType(_draggingNodeType)
                }
            };
            _nodePreview.Add(icon);
        
            // Добавляем текст
            var label = new Label(ObjectNames.NicifyVariableName(_draggingNodeType.Name))
            {
                style = {
                    position = Position.Absolute,
                    left = 30,
                    top = 10,
                    color = Color.white
                }
            };
            _nodePreview.Add(label);
        
            // Добавляем тень
            _nodePreview.AddToClassList("unity-window__header");
        
            _root.Add(_nodePreview);
        }

        private void OnGlobalMouseMove(MouseMoveEvent evt)
        {
            if (_nodePreview == null) return;
        
            // Обновляем позицию preview
            _nodePreview.style.left = evt.mousePosition.x;
            _nodePreview.style.top = evt.mousePosition.y;
        
            evt.StopPropagation();
        }
        
        private void OnGlobalMouseUp(MouseUpEvent evt)
        {
            if (evt.button != 0) return; // Только ЛКМ
        
            // Удаляем обработчики
            _root.UnregisterCallback<MouseMoveEvent>(OnGlobalMouseMove);
            _root.UnregisterCallback<MouseUpEvent>(OnGlobalMouseUp);
        
            // Завершаем операцию перетаскивания
            if (_nodePreview != null)
            {
                // Проверяем, находится ли курсор над GraphView
                if (_aIBrainGraph.contentContainer.ContainsPoint(evt.mousePosition))
                {
                    _aIBrainGraph.AddNodeComponent<TNode>(_draggingNodeType, _aIBrainGraph.contentViewContainer.WorldToLocal(evt.mousePosition));
                }
            
                // Удаляем preview
                if (_nodePreview.parent != null)
                    _nodePreview.parent.Remove(_nodePreview);
                _nodePreview = null;
            }
        
            evt.StopPropagation();
            evt.PreventDefault();
        }

        private Color GetColorForNodeType(Type type)
        {
            if (type.Name.Contains("AIAction")) return Color.green;
            if (type.Name.Contains("AIDecision")) return Color.yellow;
            return new Color(0.3f, 0.5f, 1f);
        }

        #endregion
    }
}