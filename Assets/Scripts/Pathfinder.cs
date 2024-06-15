using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour
{
    Node            m_startNode;
    public Node            m_goalNode;
    Graph           m_graph;
    GraphView       m_graphView;

    Queue<Node>         m_frontierNodes;
    List<Node>          m_exploredNodes;
    public List<Node>   m_pathNodes;

    public Color    startColor = Color.green;
    public Color    goalColor = Color.red;
    public Color    frontierColor = new Color(1f, 0f, 1f, 0.9f);
    public Color    exploredColor = new Color(0.5f, 0.5f, 0.5f, 0.9f);
    public Color    pathColor = Color.cyan;
    public Color    arrowColor = new Color32(216, 216, 216, 255);
    public Color    highlightColor = new Color32(255, 255, 128, 255);

    public bool     isComplete = false;
    private int     m_iterations = 0;

    public enum Mode
    {
        BreadthFirstSearch = 0,
        Dijkstra = 1
    }

    public bool     showIterations = true;
    public bool     showColors = true;
    public bool     showArrows = true;
    public bool     exitOnGoal = true;

    public Mode mode = Mode.BreadthFirstSearch;
    public void Init(Graph graph, GraphView graphView, Node start, Node goal)
    {
        if (start == null || goal == null || graph == null || graphView == null)
        {
            Debug.LogWarning("PATHFINDER Init error: missing component(s)!");
            return;
        }

        if (start.nodeType == NodeType.Blocked || goal.nodeType == NodeType.Blocked)
        {
            Debug.LogWarning("PATHFINDER Init error: start and goal nodes must be unblocked!");
            return;
        }

        m_graph = graph;
        m_graphView = graphView;
        m_startNode = start;
        m_goalNode = goal;

        ShowColors(graphView, start, goal);

        m_frontierNodes = new Queue<Node>();
        m_frontierNodes.Enqueue(start);

        m_exploredNodes = new List<Node>();
        m_pathNodes     = new List<Node>();

        for (int x = 0; x < m_graph.Width; x++)
        {
            for (int y = 0; y < m_graph.Height; y++)
            {
                m_graph.nodes[x, y].Reset();
            }
        }

        isComplete = false;
        m_iterations = 0;
        m_startNode.distanceTraveled = 0;
    }

    private void ShowColors()
    {
        ShowColors(m_graphView, m_startNode, m_goalNode);
    }

    private void ShowColors(GraphView graphView, Node start, Node goal)
    {
        if (graphView == null || start == null || goal == null)
        {
            return;
        }

        if (m_frontierNodes != null)
        {
            graphView.ColorNodes(m_frontierNodes.ToList(), frontierColor);
        }

        if (m_exploredNodes != null)
        {
            graphView.ColorNodes(m_exploredNodes, exploredColor);
        }

        if (m_pathNodes != null && m_pathNodes.Count > 0)
        {
            graphView.ColorNodes(m_pathNodes, pathColor);
        }

        NodeView startNodeView = graphView.nodeViews[start.xIndex, start.yIndex];

        if (startNodeView != null)
        {
            startNodeView.UpdateTileColor(startColor);
        }

        NodeView goalNodeView = graphView.nodeViews[goal.xIndex, goal.yIndex];

        if (goalNodeView != null)
        {
            goalNodeView.UpdateTileColor(goalColor);
        }
    }

    public IEnumerator SearchRoutine(float timeStep = 0.1f)
    {
        float   timeStart = Time.time;
        yield return null;

        while (!isComplete)
        {
            if (m_frontierNodes.Count > 0)
            {
                Node currentNode = m_frontierNodes.Dequeue(); /* Take the first from queue */
                m_iterations++;

                if (!m_exploredNodes.Contains(currentNode)) /* Put it inside the explored nodes container */
                {
                    m_exploredNodes.Add(currentNode);
                }

                if (mode == Mode.BreadthFirstSearch)
                {
                    ExpandFrontierBreadthFirst(currentNode);

                } else if (mode == Mode.Dijkstra)
                {
                    ExpandFrontierDijkstra(currentNode);
                }

                if (m_frontierNodes.Contains(m_goalNode))
                {
                    m_pathNodes = GetPathNodes(m_goalNode);

                    if (exitOnGoal) /* Early break if goal node reached */
                    {
                        isComplete = true;
                        Debug.Log("PATHFINDER mode: " + mode.ToString() + "    path length = " + m_goalNode.distanceTraveled.ToString());
                    }
                }

                if (showIterations)
                {
                    ShowDiagnostics();

                    yield return new WaitForSeconds(timeStep);
                }
            }
            else
            {
                isComplete = true;

                
            }
        }
        ShowDiagnostics();
        Debug.Log("PATHFINDER SearchRoutine: elapse time = " + (Time.time - timeStart).ToString() + 
                    " seconds");

    }

    private void ShowDiagnostics()
    {
        if (showColors)
        {
            ShowColors();
        }
        if (m_graphView != null && showArrows)
        {
            m_graphView.ShowNodeArrows(m_frontierNodes.ToList(), arrowColor);

            if (m_frontierNodes.Contains(m_goalNode))
            {
                m_graphView.ShowNodeArrows(m_pathNodes, highlightColor);
            }
        }
    }

    private void ExpandFrontierBreadthFirst(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i]) &&
                    !m_frontierNodes.Contains(node.neighbors[i])) /* Check if each node is newly explored */
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbor + node.distanceTraveled;
                    node.neighbors[i].distanceTraveled = newDistanceTraveled;

                    node.neighbors[i].previous = node; /* Set the current node under consideration as the previous node for all the adjacent && new nodes */
                    m_frontierNodes.Enqueue(node.neighbors[i]); /* Put the adjacent && new node processed into the queue */
                }
            }
        }
    } /* ExpandFrontierBreadthFirst */

    private void ExpandFrontierDijkstra(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i])) /* Check if each node is newly explored */
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbor + node.distanceTraveled;
                    if (float.IsPositiveInfinity(node.neighbors[i].distanceTraveled) ||
                        newDistanceTraveled < node.neighbors[i].distanceTraveled)
                    {
                        node.neighbors[i].previous = node; /* Set the current node under consideration as the previous node for all the adjacent && new nodes */
                        node.neighbors[i].distanceTraveled = newDistanceTraveled;
                    }

                    if (!m_frontierNodes.Contains(node.neighbors[i]))
                    {
                        m_frontierNodes.Enqueue(node.neighbors[i]); /* Put the adjacent && new node processed into the queue */
                    }
                }
            }
        }
    } /* ExpandFrontierDijkstra */

    public List<Node> GetPathNodes(Node endNode)
    { /* From end node get all previous nodes */
        List<Node> path = new List<Node>();
        if (endNode == null)
        {
            return path;
        }
        path.Add(endNode);

        Node currentNode = endNode.previous;

        while (currentNode != null)
        {
            path.Insert(0, currentNode);
            currentNode = currentNode.previous; 
        }

        return path;
    } /* GetPathNodes */

    public void SetModeDropdown(int modeIndex)
    {
        mode = (Mode)modeIndex;
        Debug.Log("Selected Mode: " + mode.ToString());
    }

}
