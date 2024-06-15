using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class DemoController : MonoBehaviour
{
    public MapData      mapData;
    public Graph        graph;
    public GraphView    graphView;
    public Node         startNode;
    public Node         goalNode;
    public GameObject   firstScreen; // Reference to the Initial Screen of our Game
    public GameObject   instructionsScreen;
    public Button       startButton;
    public Button       instructorButton;
    public Button       quitButton;
    public TMP_Dropdown dropdown;
    public UserPath     userPath;
    public Pathfinder   pathfinder;
    public int          startX = 0;
    public int          startY = 0;
    public int          goalX = 15;
    public int          goalY = 1;
    public float        timeStep = 0.1f;

    void Start()
    {
        if (firstScreen != null && startButton != null && instructorButton != null && quitButton != null)
        {
            firstScreen.SetActive(true);
            instructionsScreen.SetActive(false);

            startButton.onClick.AddListener(StartGame);
            instructorButton.onClick.AddListener(Instructions);
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    void Instructions()
    {
        firstScreen.SetActive(false);
        instructionsScreen.SetActive(true);

        // Ensure the back button is only active in the instructions screen
        GameObject buttonObject = GameObject.Find("BackButton");
        if (buttonObject != null)
        {
            Button localButton = buttonObject.GetComponent<Button>();
            localButton.gameObject.SetActive(true);
            localButton.onClick.AddListener(BackButtonCallback);
        }
        else
        {
            Debug.LogWarning("BackButton not found!");
        }

        GameObject buttonObject1 = GameObject.Find("BreadthFirstSearchButton");
        if (buttonObject1 != null)
        {
            Button localButton = buttonObject1.GetComponent<Button>();
            localButton.gameObject.SetActive(true);
            localButton.onClick.AddListener(BreadthFirstSearchButtonCallback);
        }
        else
        {
            Debug.LogWarning("BackButton not found!");
        }

        GameObject buttonObject2 = GameObject.Find("DijkstrasButton");
        if (buttonObject2 != null)
        {
            Button localButton = buttonObject2.GetComponent<Button>();
            localButton.gameObject.SetActive(true);
            localButton.onClick.AddListener(DijkstrasButtonCallback);
        }
        else
        {
            Debug.LogWarning("BackButton not found!");
        }

    }

    void BreadthFirstSearchButtonCallback()
    {
        Application.OpenURL("https://www.geeksforgeeks.org/breadth-first-search-or-bfs-for-a-graph/");
    }

    void DijkstrasButtonCallback()
    {
        Application.OpenURL("https://www.geeksforgeeks.org/dijkstras-shortest-path-algorithm-greedy-algo-7/");
    }

    void BackButtonCallback()
    {
        instructionsScreen.SetActive(false);
        firstScreen.SetActive(true);
    }

    void StartGame()
    {
        firstScreen.SetActive(false);

        if (mapData != null && graph != null)
        {
            int[,] mapInstance = mapData.MakeMap();
            graph.Init(mapInstance);

            graphView = graph.gameObject.GetComponent<GraphView>();

            if (graphView != null)
            {
                graphView.Init(graph);
            }

            if (graph.IsWithinBounds(startX, startY) && graph.IsWithinBounds(goalX, goalY) && pathfinder != null)
            {
                startNode = graph.nodes[startX, startY];
                goalNode = graph.nodes[goalX, goalY];

                userPath.Init(startNode, goalNode, graphView, graph);
                pathfinder.Init(graph, graphView, startNode, goalNode);

                // Subscribe to the event:
                // Whenever the GoalReached event is invoked in UserPath,
                // the OnGoalReached method in DemoController will be called.
                userPath.GoalReached += OnGoalReached;
            }
        }
    }

    void OnGoalReached()
    {
        userPath.GoalReached -= OnGoalReached; /* Unsubscribe from the event */

        StartCoroutine(pathfinder.SearchRoutine(timeStep));

        /* In Unity, StartCoroutine is a method that allows you to start a coroutine, 
         * which is a special type of function used to implement complex behaviors over time. 
         * Coroutines are particularly useful for tasks that need to occur over several frames, 
         * such as animations, procedural generation, or asynchronous operations 
         * like waiting for a period of time. */
        Debug.Log("USER DISTANCE: " + userPath.userDistance);
    }

    void QuitGame()
    {
        Application.Quit();
    }

    void LateUpdate()
    {
        if (pathfinder != null && pathfinder.isComplete) // Check if the pathfinding process is complete
        {
            List<Node> pathNodes = userPath.userPath; /* Exclude the end node */ /* Find a color for common nodes */
            if (pathNodes.Contains(goalNode)) pathNodes.Remove(goalNode);

            if (graphView != null && pathNodes != null && pathNodes.Count > 0)
            {
                graphView.ColorNodes(pathNodes, Color.yellow);
            }
            /* Call the pop-up window */
            PopupController popupController = FindObjectOfType<PopupController>();
            if (popupController != null)
            {
                popupController.ShowPopup(userPath.userDistance, pathfinder.m_goalNode.distanceTraveled); // Show the pop-up window
            }
        }
    }
}
