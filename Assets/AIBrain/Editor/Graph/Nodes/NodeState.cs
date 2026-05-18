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

        public AIPort<AIAction> AIActionPort { get; private set; }
        public  AIPort<AITransition> AITransitionPort { get; private set; }
        
        private List<AIAction> _actions = new List<AIAction>();
        private List<AITransition> _transitions = new List<AITransition>();
        
        private ListView _listViewActions;
        private ListView _listViewTransitions;
        
        public NodeState(AIBrain brain, AIState aiState) : base(brain)
        {
            AIState = aiState;
            
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
            
            _actions = AIState.Actions;
            _transitions = AIState.Transitions;
        }
        
        public override void DestroyNode()
        {
            _brain.States?.Remove(AIState);
        }

        #region Ports

        private void CreateInputPorts()
        {
            var input = Port.Create<Edge>(
                Orientation.Horizontal, 
                Direction.Input, 
                Port.Capacity.Multi,
                AIState.GetType());
            inputContainer.Add(input);
        }
        private void CreateOutputPorts()
        {
            CreateListViewActions();
            CreateListViewTransitions();
        }

        #endregion

        #region Action
        private void CreateListViewActions()
        {
            // Контейнер
            var portsContainer = new VisualElement { name = name };
            portsContainer.style.flexDirection = FlexDirection.Column;
            
            //Мульти порт
            AIActionPort = new AIPort<AIAction>(
                Direction.Output,
                Port.Capacity.Multi,
                ActionConnected,
                ActionDisconnected)
            {
                portName = "Action",
            };
            
            portsContainer.Add(AIActionPort);
            
            _listViewActions = new ListView()
            {
                // Источник данных - список портов
                itemsSource = _actions,
                
                // Шаблон для элемента списка (VisualElement, представляющий один порт)
                makeItem = MakePortItemAction,
                bindItem = BindPortItemAction,

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
        private VisualElement MakePortItemAction()
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
        private void BindPortItemAction(VisualElement element, int index)
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
        
        #region Transition
        private void CreateListViewTransitions()
        {
            // Контейнер
            var portsContainer = new VisualElement { name = name };
            portsContainer.style.flexDirection = FlexDirection.Column;
            
            //Мульти порт
            AITransitionPort = new AIPort<AITransition>(
                Direction.Output,
                Port.Capacity.Multi,
                TransitionConnected,
                TransitionDisconnected)
            {
                portName = "Transition",
            };
            
            portsContainer.Add(AITransitionPort);
            
            _listViewTransitions = new ListView()
            {
                // Источник данных - список портов
                itemsSource = _transitions,
                
                // Шаблон для элемента списка (VisualElement, представляющий один порт)
                makeItem = MakePortItemTransition,
                bindItem = BindPortItemTransition,

                // Разрешаем перетаскивание
                reorderable = true,
                showBoundCollectionSize = false, // Скрыть счетчик размера
                showFoldoutHeader = false,       // Скрыть заголовок
            };

            _listViewTransitions.itemsChosen += TransitionListViewOnitemsChosen;
            
            portsContainer.Add(_listViewTransitions);
            outputContainer.Add(portsContainer);
        }
        private void TransitionListViewOnitemsChosen(IEnumerable<object> obj)
        {
            AIState.Transitions = obj as List<AITransition>;
            _listViewTransitions.RefreshItems();
        }
        private VisualElement MakePortItemTransition()
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
        private void BindPortItemTransition(VisualElement element, int index)
        {
            if (index >= 0 && index < _transitions.Count)
            {
                var label = element.Q<Label>();
                label.text = $"{index + 1}. {_transitions[index].Label}";
            }
        }
        
        private void RefreshTransitionListView(string value)
        {
            _listViewTransitions.RefreshItems();
        }
        
        private void TransitionConnected(Edge edge)
        {
            if(edge.input.node is NodeTransition nodeTransition)
            {
                if (!_transitions.Contains(nodeTransition.AITransition))
                {
                    nodeTransition.OnChangeLableTrasition += RefreshTransitionListView;
                    _transitions.Add(nodeTransition.AITransition);
                    _listViewTransitions.RefreshItems();
                }
            }
        }
        private void TransitionDisconnected(Edge edge)
        {
            if(edge.input.node is NodeTransition nodeTransition)
            {
                if (_transitions.Contains(nodeTransition.AITransition))
                {
                    nodeTransition.OnChangeLableTrasition -= RefreshTransitionListView;
                    _transitions.Remove(nodeTransition.AITransition);
                    _listViewTransitions.RefreshItems();
                }
            }
        }
        #endregion
    }
}