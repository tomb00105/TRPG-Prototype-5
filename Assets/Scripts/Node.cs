using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private MapGraph mapGraph;
    public float entryCost = 0.5f;
    public float exitCost = 0.5f;
    public int nodeLevel;
    public bool isBlocked = false;
    public Dictionary<Node, int> adjacentNodes = new Dictionary<Node, int>();
    public List<Node> adjacentNodesList = new List<Node>();

    public float g;
    public float f;
    public float h;

    private void Awake()
    {
        mapGraph = GameObject.Find("MapGraph").GetComponent<MapGraph>();
        nodeLevel = int.Parse(transform.parent.name.ToString().Substring(transform.parent.name.ToString().Length - 1));
        foreach (KeyValuePair<Node, int> node in mapGraph.mapNodeDict)
        {
            float distance = Mathf.Abs(node.Key.transform.position.x - transform.position.x) + Mathf.Abs(node.Key.transform.position.z - transform.position.z);
            if (distance < 1.1 && distance > 0)
            {
                int levelDifference = node.Key.nodeLevel - nodeLevel;
                adjacentNodes.Add(node.Key, levelDifference);
                adjacentNodesList.Add(node.Key);
            }
        }
    }

    private void Start()
    {
        
    }
}
