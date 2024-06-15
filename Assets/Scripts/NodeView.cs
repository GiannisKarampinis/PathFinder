using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : MonoBehaviour
{
    private GameObject  tileInstance;
    public GameObject   tilePrefab; /* The prescription of how to create a tileInstance. It is set in Unity's UI */
    
    public GameObject   arrowPrefab; /* The prescription of how to create an arrowInstance. It is set in Unity's UI */
    private GameObject  arrowInstance;      /* Directional arrows during the searching mechanism */

    public Node                m_node;

    [Range(0, 0.5f)]
    public float        borderSize = 0.15f; /* Controls the padding of the maze tiles */

    public void Init(Node node)
    {
        if (tilePrefab != null)       /* If the tile prefab is set from unity's browser */
        {    
            DestroyTileInstance();    /* Destroy the existing tile instance if present */
            
            tileInstance = Instantiate(tilePrefab, transform);  /* Instantiate the tile prefab */
            tileInstance.transform.localScale = new Vector3(1f - borderSize, 1f, 1f - borderSize);

            tileInstance.transform.localPosition = Vector3.zero; /* Set the position of the tile */
           
            gameObject.name = "Node (" + node.xIndex + "," + node.yIndex + ")"; /* Set the name of the node object */

            transform.position = node.position; /* Set the position of the node object */
            
            UpdateTileColor(node.nodeType == NodeType.Blocked ? Color.black : Color.white); /* Initialize the color of the tile based on the node type */

            m_node = node; /* Keep a reference of the Node inside the NodeView */

            Collider collider = gameObject.AddComponent<BoxCollider>();

            DestroyArrow();
            arrowInstance = Instantiate(arrowPrefab, transform);
            EnableObject(arrowInstance, false); /* Do not show arrow by default */
        }
        else
        {
            Debug.LogError("Tile prefab is not assigned through Unity's Interface.");
        }
    } /* Init */

    private void DestroyTileInstance()
    {
        if (tileInstance != null)
        {
            DestroyImmediate(tileInstance);
            tileInstance = null;
        }
    } /* DestroyTileInstance */

    private void DestroyArrow()
    {
        if (arrowInstance != null)
        {
            Destroy(arrowInstance);
            arrowInstance = null;
        }
    } /* DestroyTileInstance */

    private void EnableObject(GameObject gameObject, bool state)
    {
        if (gameObject != null)
        {
            gameObject.SetActive(state);
        }
    } /* EnableObject */

    public void UpdateTileColor(Color color)
    {
        if (tileInstance != null)
        {
            Renderer tileRenderer = tileInstance.GetComponent<Renderer>();
            if (tileRenderer != null)
            {
                if (tileRenderer.material == null)
                {
                    tileRenderer.material = new Material(Shader.Find("Standard")); /* Create a default material if none is assigned */
                }

                tileRenderer.material.color = color;
            }
            else
            {
                Debug.LogError("Renderer component is missing on the Tile GameObject.");
            }
        }
        else
        {
            Debug.LogError("Tile instance is not initialized.");
        }
    } /* UpdateTileColor */

    public void UpdateArrowColor(Color color)
    {
        if (arrowInstance != null)
        {
            Renderer arrowRenderer = arrowInstance.GetComponent<Renderer>();
            if (arrowRenderer != null)
            {
                if (arrowRenderer.material == null)
                {
                    arrowRenderer.material = new Material(Shader.Find("Standard")); /* Create a default material if none is assigned */
                }

                arrowRenderer.material.color = color;
            }
            else
            {
                Debug.LogError("Renderer component is missing on the Tile GameObject.");
            }
        }
        else
        {
            Debug.LogError("Tile instance is not initialized.");
        }
    } /* UpdateArrowColor */

    public void ShowArrow(Color color)
    {
        if (m_node.previous != null && arrowInstance != null && m_node.previous != null)
        {
            EnableObject(arrowInstance, true);

            Vector3 dirToPrevious = (m_node.previous.position - m_node.position).normalized; /* Vector from cog of current node to previous */
            arrowInstance.transform.rotation = Quaternion.LookRotation(dirToPrevious);      /* Transform-rotate the arrow */

            UpdateArrowColor(color);
        }

    } /* ShowArrow */

} /* NodeView */
