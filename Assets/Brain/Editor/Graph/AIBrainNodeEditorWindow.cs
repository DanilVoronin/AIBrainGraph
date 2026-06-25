using Brain.Graph.GUI;
using Brain.Graph.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Brain.Graph
{
    public class AIBrainNodeEditorWindow : EditorWindow
    {
        private AIBrainGraph _AIBrainGraph;
        private GameObject _targetGameObject;
        
        // [MenuItem("AIBrain/AIBrainGraph")]
        public static void ShowWindow() => GetWindow<AIBrainNodeEditorWindow>("AIBrain");

        public void CreateGUI()
        {
            _targetGameObject = Selection.activeGameObject;
            CreateUI();
        }

        private void CreateUI()
        {
            // Основной разделитель: левая панель (ноды) + правая (граф)
            var splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(splitView);
            splitView.style.flexGrow = 1;

            // ЛЕВАЯ ПАНЕЛЬ: список нод
            var panal = new VisualElement { name = "panal" };
            panal.style.flexDirection = FlexDirection.Column;
            splitView.Add(panal);
            
            //Общее для всех нодов
            var nodePalette = new ScrollView();
            nodePalette.name = "node-palette";
            nodePalette.style.paddingLeft = 5;
            nodePalette.style.paddingRight = 5;
            panal.Add(nodePalette);

            CreateGraph(splitView);

            nodePalette.Add(new AIListNodes<AIAction, NodeAction>("Actions", _AIBrainGraph, rootVisualElement));
            nodePalette.Add(new AIListNodes<AIDecision, NodeDecision>("Decision", _AIBrainGraph, rootVisualElement));
        }
        
        private void OnDestroy()
        {
            _AIBrainGraph.Close();
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }
        
        private void OnUndoRedoPerformed()
        {
            rootVisualElement.Clear();
            CreateUI();
        }

        private void CreateGraph(TwoPaneSplitView splitView)
        {
            var grapg = new VisualElement { name = "grapg" };
            grapg.style.flexDirection = FlexDirection.Column;
            splitView.Add(grapg);

            // ПРАВАЯ ПАНЕЛЬ: граф
            _AIBrainGraph = new AIBrainGraph(_targetGameObject);
            _AIBrainGraph.StretchToParentSize();
            grapg.Add(_AIBrainGraph);
            
            // Поисковая строка
            //var searchField = new TextField();
            //searchField.RegisterValueChangedCallback(evt => 
            //    _nodeListView.Q<TextField>().SetValueWithoutNotify(evt.newValue));
            //nodePalette.Add(searchField);
        }
    }
}