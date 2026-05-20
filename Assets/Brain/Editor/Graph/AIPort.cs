using System;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace  Brain.Graph
{
    public class AIPort<T> : Port
    {
        // Делегат для уведомления о изменениях
        private readonly Action<Edge> _connected;
        private readonly Action<Edge> _disconnected;

        public AIPort(
            Direction direction,
            Capacity capacity,
            Action<Edge> connected,
            Action<Edge> disconnected)
            : base(Orientation.Horizontal, direction, capacity, typeof(T))
        {
            _connected =  connected;
            _disconnected = disconnected;
            
            var listener = new DefaultEdgeConnectorListener();
            var edgeConnector = new EdgeConnector<Edge>(listener);
            this.AddManipulator(edgeConnector);
        }

        public override void Connect(Edge edge)
        {
            base.Connect(edge);
            _connected?.Invoke(edge);
        }

        public override void Disconnect(Edge edge)
        {
            base.Disconnect(edge);
            _disconnected?.Invoke(edge);
        }
    }
}