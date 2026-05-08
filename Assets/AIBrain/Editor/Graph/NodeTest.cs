using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Brain.Graph
{
    public class NodeTest<T> : Node
    {
        private AIBrain _brain;
        private readonly Toggle _toggle;

        public NodeTest(string title, AIBrain brain) : base()
        {
            this.title = title;
            _brain = brain;
            
            //capabilities |= Capabilities.Deletable | Capabilities.Selectable | Capabilities.Movable;
            capabilities |= Capabilities.Selectable |
                            //Capabilities.Collapsible |
                            //Capabilities.Resizable |
                            Capabilities.Movable |
                            Capabilities.Deletable;
                            //Capabilities.Droppable |
                            //Capabilities.Ascendable |
                            //Capabilities.Renamable |
                            //Capabilities.Copiable |
                            //Capabilities.Snappable |
                            //Capabilities.Groupable |
                            //Capabilities.Stackable;

            CreatePorts();
            
            viewDataKey = _brain.GetInstanceID().ToString();
            _toggle = new Toggle("BrainActive");
            _toggle.RegisterCallback<ChangeEvent<bool>>(evt => _brain.BrainActive = evt.newValue);
            mainContainer.Add(_toggle);
            UpdateDisplay();
        }

        private void CreatePorts()
        {
            System.Reflection.FieldInfo[] fields = 
                typeof(T).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var item in fields)
            {
                CreatePort(item.Name, item.FieldType);
            }
            
            
        }

        private void CreatePort(string name, Type type)
        {
            var input = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,type);
            input.portName = name;
            inputContainer.Add(input);
            
            Toggle toggle = new Toggle();
            
            input.Add(toggle);
            
            
            var output = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, type);
            output.portName = name;
            outputContainer.Add(output);   
        }

        public void UpdateDisplay()
        {
            if (_brain != null)
            {
                _toggle.value = _brain.BrainActive;
            }
        }
    }
}