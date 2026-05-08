using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Brain.Graph.Nodes
{
    /// <summary>
    /// Нод состояния, соединяется с мозгами
    /// </summary>
    public class NodeState : AINode
    {
        public readonly AIState AIState;
        
        private readonly AIBrain _brain;

        private  AIPort<AIAction> _actionPort;
        private  AIPort<AITransition> _transitionPort;
        
        private List<AIAction> _actions = new List<AIAction>();
        private List<AITransition> _transitions = new List<AITransition>();
        
        private ListView _listViewActions;
        
        public NodeState(AIBrain brain)
        {
            AIState = new AIState();
            _brain = brain;
            
            brain.States?.Add(AIState);
            
            title = "State";

            CreateInputPorts();
            CreateOutputPorts();
            
            var textField = new TextField
            {
                value = AIState.StateName,
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    width = 100,
                }
            };

            textField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                AIState.StateName = evt.newValue;
            });
            
            titleContainer.Add(textField);
            
            AIState.Actions = _actions;
        }
        
        protected override void OnDetachFromPanelEvent(DetachFromPanelEvent e)
        {
            _brain.States?.Remove(AIState);
        }

        #region Ports

        private void CreateInputPorts()
        {
            var input = Port.Create<Edge>(
                Orientation.Horizontal, 
                Direction.Input, 
                Port.Capacity.Single,
                AIState.GetType());
            inputContainer.Add(input);
        }
        private void CreateOutputPorts()
        {
            CreateListViewActions();
        }

        #endregion

        #region Action
        private void CreateListViewActions()
        {
            // Контейнер
            var portsContainer = new VisualElement { name = "Actions" };
            portsContainer.style.flexDirection = FlexDirection.Column;
            
            //Мульти порт
            _actionPort = new AIPort<AIAction>(
                Direction.Output,
                Port.Capacity.Multi,
                ActionConnected,
                ActionDisconnected)
            {
                portName = "Action",
            };
            
            portsContainer.Add(_actionPort);
            
            _listViewActions = new ListView()
            {
                // Источник данных - список портов
                itemsSource = _actions,
                
                // Шаблон для элемента списка (VisualElement, представляющий один порт)
                makeItem = MakePortItem,
                bindItem = BindPortItem,

                // Разрешаем перетаскивание
                reorderable = true,
                showBoundCollectionSize = false, // Скрыть счетчик размера
                showFoldoutHeader = false,       // Скрыть заголовок
            };

            _listViewActions.itemsChosen += ActionsListViewOnitemsChosen;
            
            portsContainer.Add(_listViewActions);
            outputContainer.Add(portsContainer);
        }
        private void ActionsListViewOnitemsChosen(IEnumerable<object> obj)
        {
            AIState.Actions = obj as List<AIAction>;
            _listViewActions.RefreshItems();
        }
        private VisualElement MakePortItem()
        {
            var item = new VisualElement();
            item.style.flexDirection = FlexDirection.Row;
            item.style.alignItems = Align.Center;

            Label nameLabel = new Label();
            nameLabel.style.flexGrow = 1;
            nameLabel.AddManipulator(new Dragger());
            
            item.Add(nameLabel);

            item.AddManipulator(new Dragger());
            
            return item;
        }
        private void BindPortItem(VisualElement element, int index)
        {
            if (index >= 0 && index < _actions.Count)
            {
                var label = element.Q<Label>();
                label.text = $"{index + 1}. {_actions[index].Label}";
            }
        }
        
        private void RefreshActionsListView(string value)
        {
            _listViewActions.RefreshItems();
        }
        
        private void ActionConnected(Edge edge)
        {
            if(edge.input.node is NodeAction nodeAction)
            {
                if (!_actions.Contains(nodeAction.AIAction))
                {
                    nodeAction.OnChangeLableAction += RefreshActionsListView;
                    _actions.Add(nodeAction.AIAction);
                    _listViewActions.RefreshItems();
                }
            }
        }
        private void ActionDisconnected(Edge edge)
        {
            if(edge.input.node is NodeAction nodeAction)
            {
                if (_actions.Contains(nodeAction.AIAction))
                {
                    nodeAction.OnChangeLableAction -= RefreshActionsListView;
                    _actions.Remove(nodeAction.AIAction);
                    _listViewActions.RefreshItems();
                }
            }
        }
        #endregion
    }
}