using UnityEditor;
using UnityEngine.UIElements;

namespace Brain.Graph
{
    public class AIBrainNodeEditorWindow : EditorWindow
    {
        AIBrainGraph _AIBrainGraph;
        
        [MenuItem("AIBrain/AIBrainGraph")]
        public static void ShowWindow() => GetWindow<AIBrainNodeEditorWindow>("AIBrain");

        public void CreateGUI()
        {
            _AIBrainGraph = new AIBrainGraph();
            _AIBrainGraph.StretchToParentSize();
            rootVisualElement.Add(_AIBrainGraph);
        }
    }
}