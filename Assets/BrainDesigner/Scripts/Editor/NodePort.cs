#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace BrainDesigner.Scripts.Editor
{
    public class NodePort : Port
    {
        class DefaultEdgeConnectorListener : IEdgeConnectorListener
        {
            readonly GraphViewChange graphViewChange;
            readonly List<Edge> edgesToCreate;
            readonly List<GraphElement> elementsToDelete;


            internal DefaultEdgeConnectorListener()
            {
                this.edgesToCreate = new List<Edge>();
                this.elementsToDelete = new List<GraphElement>();

                this.graphViewChange.edgesToCreate = this.edgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position) { }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                this.edgesToCreate.Clear();
                this.edgesToCreate.Add(edge);
                this.elementsToDelete.Clear();

                if (edge.input.capacity == Capacity.Single)
                    foreach (Edge edgeToDelete in edge.input.connections)
                        if (edgeToDelete != edge)
                            this.elementsToDelete.Add(edgeToDelete);
                if (edge.output.capacity == Capacity.Single)
                    foreach (Edge edgeToDelete in edge.output.connections)
                        if (edgeToDelete != edge)
                            this.elementsToDelete.Add(edgeToDelete);
                if (this.elementsToDelete.Count > 0)
                    graphView.DeleteElements(this.elementsToDelete);

                var edgesToCreate = this.edgesToCreate;
                if (graphView.graphViewChanged != null)
                    edgesToCreate = graphView.graphViewChanged(this.graphViewChange).edgesToCreate;

                foreach (Edge e in edgesToCreate)
                {
                    graphView.AddElement(e);
                    edge.input.Connect(e);
                    edge.output.Connect(e);
                }
            }
        }

        internal NodePort(Direction direction, Capacity capacity) : base(Orientation.Vertical, direction, capacity,
            typeof(bool))
        {
            var connectorListener = new DefaultEdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<Edge>(connectorListener);
            this.AddManipulator(m_EdgeConnector);

            style.flexGrow = 1f;
            style.opacity = 0f;
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            Rect rect = new Rect(0, 0, layout.width, layout.height);
            return rect.Contains(localPoint);
        }
    }
}
#endif