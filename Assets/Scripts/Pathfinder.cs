using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour
{
    private MapGraph mapGraph;
    private EnemyController enemyController;
    public Dictionary<Node, float> nodesInRange = new Dictionary<Node, float>();
    public Dictionary<Node, float> dijkstraDistDict = new Dictionary<Node, float>();
    public Dictionary<Node, Node> dijkstraPathDict = new Dictionary<Node, Node>();
    public List<Node> path = new List<Node>();

    public Node currentNode;

    private void Awake()
    {
        mapGraph = GameObject.Find("MapGraph").GetComponent<MapGraph>();
        enemyController = GetComponent<EnemyController>();
    }

    private void Start()
    {
        currentNode = GetCurrentNode();
    }

    public bool Dijkstra()
    {
        currentNode = GetCurrentNode();
        nodesInRange.Clear();
        dijkstraDistDict.Clear();
        dijkstraPathDict.Clear();
        path.Clear();
        GetNodesInRange();
        Node priorityNode;
        PriorityQueue<Node> priorityQueue = new PriorityQueue<Node>();
        dijkstraDistDict.Add(currentNode, 0);
        priorityQueue.Enqueue(0, currentNode);
        foreach (Node node in nodesInRange.Keys)
        {
            Debug.Log("Node: " + node.name);
            dijkstraDistDict.Add(node, nodesInRange[node]);
            dijkstraPathDict.Add(node, null);
            priorityQueue.Enqueue(nodesInRange[node], node);
        }
        
        Debug.Log("Priority Queue Count: " + priorityQueue.Count);
        while (priorityQueue.Count > 0)
        {
            priorityNode = priorityQueue.Dequeue();
            if (priorityQueue.Count > 0)
            {
                Debug.Log("Priority Queue[0]: " + priorityQueue.queue[0].Object.name);
            }
            Debug.Log("Priority Node: " + priorityNode.name);
            Debug.Log("Adjacent Node Count: " + priorityNode.adjacentNodes.Keys.Count);
            foreach (KeyValuePair<Node, int> adjacentNode in priorityNode.adjacentNodes)
            {
                Debug.Log("Adjacent Node: " + adjacentNode.Key.name + " Level Difference: " + adjacentNode.Value.ToString());
                if (Mathf.Abs(adjacentNode.Value) <= enemyController.jump && !adjacentNode.Key.isBlocked)
                {
                    float newDist = dijkstraDistDict[priorityNode] + priorityNode.exitCost + adjacentNode.Key.entryCost;
                    Debug.Log("New Distance: " + newDist);
                    Debug.Log("Current Distance: " + dijkstraDistDict[adjacentNode.Key]);
                    if (newDist <= dijkstraDistDict[adjacentNode.Key])
                    {
                        dijkstraDistDict[adjacentNode.Key] = newDist;
                        dijkstraPathDict[adjacentNode.Key] = priorityNode;
                        priorityQueue.UpdatePriority(adjacentNode.Key, newDist);
                        Debug.Log("New Distance Updated");
                    }
                }
            }
            Debug.Log("Priority Queue Count Second Check: " + priorityQueue.Count);
            if (priorityQueue.Count > 0)
            {
                Debug.Log("Next Priority Node: " + priorityQueue.queue[0].Object.name + " Priority: " + priorityQueue.queue[0].Priority);
            }
        }
        if (priorityQueue.Count > 0)
        {
            if (priorityQueue.queue[0].Priority == Mathf.Infinity)
            {
                Debug.Log("Cannot move");
                return false;
            }
        }
        return true;
    }

    private bool GetNodesInRange()
    {
        foreach (KeyValuePair<Node, int> node in mapGraph.mapNodeDict)
        {
            float distance = Mathf.Abs(node.Key.transform.position.x - transform.position.x) + Mathf.Abs(node.Key.transform.position.z - transform.position.z);
            Debug.Log("Distance: " + distance);
            int levelDifference = node.Value - currentNode.nodeLevel;
            Debug.Log("Moves: " + enemyController.moves);
            Debug.Log("Blocked: " + node.Key.isBlocked.ToString());
            Debug.Log((distance > 0 && distance <= enemyController.moves && !node.Key.isBlocked).ToString());
            if (distance > 0 && distance <= enemyController.moves && !node.Key.isBlocked)
            {
                Debug.Log("Level Difference: " + levelDifference + "Weighted Distance: " + distance * enemyController.jump);
                if (levelDifference <= distance * enemyController.jump)
                {
                    if (!nodesInRange.Keys.Contains(node.Key))
                    {
                        nodesInRange.Add(node.Key, distance);
                    }
                }
            }
            else
            {
                node.Key.isBlocked = true;
            }
        }
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy Character"))
        {
            Node node = GetNodeFromGameObject(nodesInRange.Keys.ToList(), enemy);
            if (nodesInRange.Keys.ToList().Contains(node))
            {
                nodesInRange.Remove(node);
            }
        }
        if (nodesInRange.Count == 0 || nodesInRange == null)
        {
            Debug.Log("No nodes found in range");
            return false;
        }
        return true;
    }

    
    public bool BuildPath(Node targetNode)
    {
        Node u = targetNode;
        path.Clear();
        while (u != currentNode)
        {
            path.Add(u);
            u = dijkstraPathDict[u];
        }
        path.Reverse();
        return true;
    }

    private Node GetCurrentNode()
    {
        foreach (Node node in mapGraph.mapNodeDict.Keys)
        {
            if (node.transform.position.x == transform.position.x && node.transform.position.z == transform.position.z)
            {
                return node;
            }
        }
        Debug.Log("Could not find current node!");
        return null;
    }

    public Node GetNodeFromGameObject(List<Node> graph, GameObject gameObjectToTest)
    {
        foreach (Node node in graph)
        {
            if (node.transform.position.x == gameObjectToTest.transform.position.x && node.transform.position.z == gameObjectToTest.transform.position.z)
            {
                return node;
            }
        }
        return null;
    }
}

public class PriorityQueue<T>
{
    public class Item
    {
        public float Priority { get; set; }
        public T Object { get; set; }
    }

    //object array
    public List<Item> queue = new List<Item>();
    int heapSize = -1;
    bool _isMinPriorityQueue = true;
    public int Count { get { return queue.Count; } }

    /// <summary>
    /// If min queue or max queue
    /// </summary>
    /// <param name="isMinPriorityQueue"></param>
    public PriorityQueue(bool isMinPriorityQueue = false)
    {
        _isMinPriorityQueue = isMinPriorityQueue;
    }


    /// <summary>
    /// Enqueue the object with priority
    /// </summary>
    /// <param name="priority"></param>
    /// <param name="obj"></param>
    public void Enqueue(float priority, T obj)
    {
        Item node = new Item() { Priority = priority, Object = obj };
        queue.Add(node);
        heapSize++;
        //Maintaining heap
        if (_isMinPriorityQueue)
            BuildHeapMin(heapSize);
        else
            BuildHeapMax(heapSize);
    }

    /// <summary>
    /// Dequeue the object
    /// </summary>
    /// <returns></returns>
    public T Dequeue()
    {
        if (heapSize > -1)
        {
            var returnVal = queue[0].Object;
            queue[0] = queue[heapSize];
            queue.RemoveAt(heapSize);
            heapSize--;
            //Maintaining lowest or highest at root based on min or max queue
            if (_isMinPriorityQueue)
                MinHeapify(0);
            else
                MaxHeapify(0);
            return returnVal;
        }
        else
            Debug.LogError("Queue is empty");
            return default(T);
    }
    /// <summary>
    /// Updating the priority of specific object
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="priority"></param>
    public void UpdatePriority(T obj, float priority)
    {
        int i = 0;
        for (; i <= heapSize; i++)
        {
            Item node = queue[i];
            if (object.ReferenceEquals(node.Object, obj))
            {
                node.Priority = priority;
                if (_isMinPriorityQueue)
                {
                    BuildHeapMin(i);
                    MinHeapify(i);
                }
                else
                {
                    BuildHeapMax(i);
                    MaxHeapify(i);
                }
            }
        }
    }
    /// <summary>
    /// Searching an object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool IsInQueue(T obj)
    {
        foreach (Item node in queue)
            if (object.ReferenceEquals(node.Object, obj))
                return true;
        return false;
    }

    private void MaxHeapify(int i)
    {
        int left = ChildL(i);
        int right = ChildR(i);

        int heighst = i;

        if (left <= heapSize && queue[heighst].Priority < queue[left].Priority)
            heighst = left;
        if (right <= heapSize && queue[heighst].Priority < queue[right].Priority)
            heighst = right;

        if (heighst != i)
        {
            Swap(heighst, i);
            MaxHeapify(heighst);
        }
    }
    private void MinHeapify(int i)
    {
        int left = ChildL(i);
        int right = ChildR(i);

        int lowest = i;

        if (left <= heapSize && queue[lowest].Priority > queue[left].Priority)
            lowest = left;
        if (right <= heapSize && queue[lowest].Priority > queue[right].Priority)
            lowest = right;

        if (lowest != i)
        {
            Swap(lowest, i);
            MinHeapify(lowest);
        }
    }

    /// <summary>
    /// Maintain max heap
    /// </summary>
    /// <param name="i"></param>
    private void BuildHeapMax(int i)
    {
        while (i >= 0 && queue[(i - 1) / 2].Priority < queue[i].Priority)
        {
            Swap(i, (i - 1) / 2);
            i = (i - 1) / 2;
        }
    }
    /// <summary>
    /// Maintain min heap
    /// </summary>
    /// <param name="i"></param>
    private void BuildHeapMin(int i)
    {
        while (i >= 0 && queue[(i - 1) / 2].Priority > queue[i].Priority)
        {
            Swap(i, (i - 1) / 2);
            i = (i - 1) / 2;
        }
    }

    private void Swap(int i, int j)
    {
        var temp = queue[i];
        queue[i] = queue[j];
        queue[j] = temp;
    }
    private int ChildL(int i)
    {
        return i * 2 + 1;
    }
    private int ChildR(int i)
    {
        return i * 2 + 2;
    }
}

