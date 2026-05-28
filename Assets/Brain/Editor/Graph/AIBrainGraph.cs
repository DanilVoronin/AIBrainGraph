using System.Collections.Generic;
using Brain.Graph.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

namespace Brain.Graph
{
    public class AIBrainGraph : GraphView
    {
        public Vector2 worldMousePosition;
        
        private AIBrain _brain;
        private float startX = 250, startY = 250;
        
        private NodeBrain _nodeBrain;
        
        public AIBrainGraph()
        {
            CreateGraph();
            InitGraph(Selection.activeObject);
            
            Load(_brain, _nodeBrain);
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
                base.BuildContextualMenu(evt);
                
                var toRemove = evt.menu.MenuItems()
                    .OfType<DropdownMenuAction>()
                    .Where(dmi => new[] { "Cut", "Delete" }.Contains(dmi.name))
                    .ToList();

                foreach (var item in toRemove)
                    evt.menu.MenuItems().Remove(item);
                
                evt.menu.AppendSeparator();
                
                //evt.menu.ClearItems();
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

        /// <summary>
        /// Создает компонентый нод 
        /// </summary>
        /// <param name="position">Позиция нода</param>
        /// <typeparam name="TNode">Тип нода</typeparam>
        public void AddNodeComponent<TNode>(Type component, Vector2 position)  
            where TNode : AINodeComponent, new()
        {
            var node = new TNode
            {
                title = component.Name
            };
            node.Setup(_brain, component);
            
            AddElement(node);
            node.SetPosition(new Rect(position.x - 100, position.y - 50, 200, 150));
        }

        private void AddState(Vector2 position)
        {
            AIState state = new AIState();
            _brain.States?.Add(state);
            
            var node = new NodeState(_brain, state);
            AddElement(node);

            node.SetPosition(new Rect(position.x - 100, position.y - 50, 200, 150));
        }
            
        private void AddTransitionNode(Vector2 position)
        {
            var node = new NodeTransition(_brain, new AITransition());
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
                    _nodeBrain = new NodeBrain(_brain);
                    
                    AddElement(_nodeBrain);
                    
                    _nodeBrain.SetPosition(new Rect(startX, startY, 200, 150));
                }
            }
        }
        
        #endregion Init

        #region Load

        private void Load(AIBrain brain, NodeBrain nodeBrain)
        {
            //Ищем на объекте все действия и условия перехода,
            //Если какие объекты не настроены в brain но есть на игровом объекте, отрисовываем их отдельно
            List<AIAction> actions = brain.GetComponentsInChildren<AIAction>(true).ToList();
            List<AIDecision> decisions = brain.GetComponentsInChildren<AIDecision>(true).ToList();
            
            List<NodeTransition> nodeTransitions = new ();
            List<NodeState> nodeStates = new List<NodeState>();

            float x = startX + 300;
            float y = startY;

            foreach (var state in _brain.States)
            {
                y = startY;
                
                if(state is null) continue;
                
                NodeState nodeState = CreateNodeState(state);
                nodeStates.Add(nodeState);
                nodeState.SetPosition(new Rect(x, y, 200, 150));

                if (brain.FirstState == state)
                {
                    AddElement(new Edge()
                    {
                        output = nodeBrain.AIStatePort,
                        input = nodeState.AIInputStatePort
                    });
                }
                
                x += 500;
                
                foreach (var action in state.Actions)
                {
                    if(actions.Contains(action)) actions.Remove(action);
                    
                    NodeAction nodeAction = CreateNodeAction(action);
                    
                    nodeAction.SetPosition(new Rect(x, y, 200, 150));
                    y += 300;
                    
                    Edge edge = new Edge()
                    {
                        output = nodeState.AIActionPort,
                        input = nodeAction.AIStatePort
                    };
                    AddElement(edge);
                }

                foreach (var transition in state.Transitions)
                {
                    
                    NodeTransition nodeTransition = CreateNodeTransition(transition);
                    nodeTransition.SetPosition(new Rect(x, y, 200, 150));
                    
                    nodeTransitions.Add(nodeTransition);
                    
                    AddElement(new Edge()
                    {
                        output = nodeState.AITransitionPort,
                        input = nodeTransition.AITransitionPort
                    });

                    if (transition.Decision != null)
                    {
                        if(decisions.Contains(transition.Decision)) decisions.Remove(transition.Decision);
                       
                        NodeDecision nodeDecision = CreateNodeDecision(transition.Decision);
                        nodeDecision.SetPosition(new Rect(x + 300, y, 200, 150));
                        AddElement(nodeDecision);
                        
                        AddElement(new Edge()
                        {
                            output = nodeTransition.AIDecisionPort,
                            input = nodeDecision.AITransitionPort
                        });
                    }
                    y += 300;
                }
                x += 700;
                
            }
            
            foreach (var nodeTransition in nodeTransitions)
            {
                CreateEdgeTransitionToState(
                    _brain.GetAIStateByIndex(nodeTransition.AITransition.StateTrueIndex),
                    nodeStates, 
                    nodeTransition.TruePort);
                
                CreateEdgeTransitionToState(
                    _brain.GetAIStateByIndex(nodeTransition.AITransition.StateFalseIndex),
                    nodeStates, 
                    nodeTransition.FalsePort);
            }
            
            //Создаем оставшиеся не связанные ноды
            //Так как на этом этапе неполучается получить размер элемента, в следующем кадре обновим положение
            //Но пока так 😋

            x = startX - 500; 
            y = startY;
            
            foreach (var action in actions)
            {
                NodeAction node = CreateNodeAction(action);
                node.SetPosition(new Rect(x, y, 200, 150));
                y += 300;
            }
            
            foreach (var decision in decisions)
            {
                NodeDecision node = CreateNodeDecision(decision);
                node.SetPosition(new Rect(x, y, 200, 150));
                y += 300;
            }
        }

        private void CreateEdgeTransitionToState(
            AIState state,
            List<NodeState> nodeStates,
            Port transitionPort)
        {
            if (state != null)
            {
                NodeState nodeState = nodeStates.FirstOrDefault(node => node.AIState == state);
                if (nodeState != null)
                {
                    AddElement(new Edge()
                    {
                        output = transitionPort,
                        input = nodeState.AIInputStatePort
                    });
                }
            }
        }

        private NodeState CreateNodeState(AIState state)
        {
            var nodeState = new NodeState(_brain, state);
            AddElement(nodeState);
            return nodeState;
        }

        private NodeAction CreateNodeAction(AIAction action)
        {
            NodeAction nodeAction = new NodeAction();
            nodeAction.Setup(_brain, action);
            nodeAction.title = action.GetType().Name;
            AddElement(nodeAction);
            return nodeAction;
        }

        private NodeTransition CreateNodeTransition(AITransition transition)
        {
            NodeTransition nodeTransition = new NodeTransition(_brain, transition);
            AddElement(nodeTransition);
            return nodeTransition;
        }

        private NodeDecision CreateNodeDecision(AIDecision decision)
        {
            NodeDecision nodeDecision = new NodeDecision();
            nodeDecision.Setup(_brain, decision);
            nodeDecision.title = decision.GetType().Name;
            AddElement(nodeDecision);
            return nodeDecision;
        }

        #endregion

        public void Close()
        {
            UnregisterCallback<MouseDownEvent>(MouseDownEvent);
        }
    }
}