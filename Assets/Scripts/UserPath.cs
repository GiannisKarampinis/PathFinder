using System.Collections.Generic;
using UnityEngine;
using System;

public class UserPath : MonoBehaviour
{
    public GraphView        graphView;
    public Graph            graph;
    public List<Node>      userPath = new List<Node>();
    private Node            startNode;
    private Node            goalNode;
    private bool            selectionModeEnabledFlag = false;
    public event Action     GoalReached;
    public float            userDistance = 0.0f;

    public void Init(Node start, Node goal, GraphView graphView, Graph graph)
    {
        startNode       = start;
        goalNode        = goal;
        this.graphView  = graphView;
        this.graph      = graph;

        userPath.Clear();
        selectionModeEnabledFlag = true;
    }

    void Update()
    {
        if (selectionModeEnabledFlag && Input.GetMouseButtonDown(0))
        {
            Ray             ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[]    hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                /* Checks if NodeView exists and if the node type (nodeView.m_node.nodeType) is not Blocked. */
                NodeView nodeView = hit.collider.GetComponent<NodeView>();

                if (nodeView != null && nodeView.m_node.nodeType != NodeType.Blocked)
                {
                    Node selectedNode = nodeView.m_node;

                    /* The previous inserted node in the user path has to be a neighbor of the current node */
                    /* TODO: Demand the first node of selection to be neighbor of the start node */
                    if (selectedNode != startNode &&
                        (userPath.Count == 0 || 
                        userPath[userPath.Count - 1].neighbors.Contains(selectedNode))
                       )
                    {
                        if (userPath.Count > 0)
                        {
                            Node prevNode = userPath[userPath.Count - 1];
                            
                            float distanceToNeighbor = graph.GetNodeDistance(selectedNode, prevNode);
                            userDistance += distanceToNeighbor;
                        }
                        userPath.Add(selectedNode);
                        graphView.ColorNodes(userPath, Color.yellow);

                        if (selectedNode == goalNode) /* End of destination */
                        {
                            selectionModeEnabledFlag = false;
                            OnGoalReached();
                        }
                        break;
                    }
                }
            }
        }
    }

    protected virtual void OnGoalReached()
    {
        GoalReached?.Invoke(); /* Notifies all the subscribers */
    }

}