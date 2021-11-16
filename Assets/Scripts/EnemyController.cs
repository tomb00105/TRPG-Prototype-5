using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyBehaviour enemyBehaviour;
    [SerializeField] private MapGraph mapGraph;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private TurnManager turnManager;
    public BaseCharacter baseCharacter;

    public int moves;
    public int jump = 2;
    public bool isTurn = false;
    public bool moveComplete = false;
    public bool actionComplete = false;
    public int nextNode = 0;

    public Dictionary<BaseCharacter, float> characterPriorityDict = new Dictionary<BaseCharacter, float>();
    public Dictionary<string, bool> currentState = new Dictionary<string, bool>();
    public Dictionary<Node, float> positionBonus = new Dictionary<Node, float>();


    private void Start()
    {
        moves = baseCharacter.moves;
        mapGraph = GameObject.Find("MapGraph").GetComponent<MapGraph>();
        enemyBehaviour = GetComponent<EnemyBehaviour>();
        pathfinder = GetComponent<Pathfinder>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Player Character"))
        {
            //NEED TO CALCULATE INITIAL PRIORITY HERE
            float basePriority = 1;
            characterPriorityDict.Add(character.GetComponent<PlayerCharacterController>().baseCharacter, basePriority);
        }
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Enemy Character"))
        {
            //NEED TO CALCULATE INITIAL PRIORITY HERE
            float basePriority = 1;
            characterPriorityDict.Add(character.GetComponent<EnemyController>().baseCharacter, basePriority);
        }
        PositionBonus();
    }

    private void Update()
    {
        if (!isTurn)
        {
            return;
        }
        Debug.Log("Move then act: " + enemyBehaviour.actionPriorityList[0].moveThenAct);
        if (enemyBehaviour.actionPriorityList[0].moveThenAct == true)
        {
            Debug.Log("Move complete: " + moveComplete);
            if (!moveComplete)
            {
                Debug.Log("Node to move to: " + enemyBehaviour.actionPriorityList[0].node.name);
                transform.position = new Vector3(enemyBehaviour.actionPriorityList[0].node.transform.position.x, enemyBehaviour.actionPriorityList[0].node.transform.position.y + 0.54f, enemyBehaviour.actionPriorityList[0].node.transform.position.z);
                moveComplete = true;
                //MoveTo(enemyBehaviour.actionPriorityList[0].node);
            }
            if (moveComplete && !actionComplete)
            {
                enemyBehaviour.actionPriorityList[0].action.TakeAction();
                actionComplete = true;
            }
            if (moveComplete && actionComplete)
            {
                turnManager.EndTurn();
            }
        }
        if (enemyBehaviour.actionPriorityList[0].moveThenAct == false)
        {
            if (!actionComplete)
            {
                enemyBehaviour.actionPriorityList[0].action.TakeAction();
                actionComplete = true;
            }
            if (actionComplete && !moveComplete)
            {
                MoveTo(enemyBehaviour.actionPriorityList[0].node);
                moveComplete = true;
            }
            if (moveComplete && actionComplete)
            {
                turnManager.EndTurn();
            }
        }
    }

    public void TurnStart()
    {
        moveComplete = false;
        actionComplete = false;
        nextNode = 0;
        moves = baseCharacter.moves;
        pathfinder.Dijkstra();
        enemyBehaviour.GetActionPriority();
        Debug.Log("Actions in priority List: " + enemyBehaviour.actionPriorityList.Count);
        pathfinder.BuildPath(enemyBehaviour.actionPriorityList[0].node);
        isTurn = true;
    }

    public void MoveTo(Node node)
    {
        if (!transform.position.Equals(new Vector3(pathfinder.path[pathfinder.path.Count - 1].transform.position.x, pathfinder.path[pathfinder.path.Count - 1].transform.position.y + 0.54f, pathfinder.path[pathfinder.path.Count - 1].transform.position.z)))
        {
            Vector3.MoveTowards(transform.position, new Vector3(pathfinder.path[nextNode].transform.position.x, pathfinder.path[nextNode].transform.position.y + 0.54f, pathfinder.path[nextNode].transform.position.z), 1f);
            if (transform.position.Equals(new Vector3(pathfinder.path[nextNode].transform.position.x, pathfinder.path[nextNode].transform.position.y + 0.54f, pathfinder.path[nextNode].transform.position.z)))
            {
                if (nextNode <= pathfinder.path.Count)
                {
                    nextNode++;
                }
            }
        }
        if (transform.position.Equals(new Vector3(pathfinder.path[pathfinder.path.Count - 1].transform.position.x, pathfinder.path[pathfinder.path.Count - 1].transform.position.y + 0.54f, pathfinder.path[pathfinder.path.Count - 1].transform.position.z)))
        {
            moveComplete = true;
            nextNode = 0;
        }
    }
    public void MoveTo(Node node, GameObject targetToFace)
    {
        Debug.Log("Move Started");
        if (!transform.position.Equals(new Vector3(pathfinder.path[pathfinder.path.Count - 1].transform.position.x, pathfinder.path[pathfinder.path.Count - 1].transform.position.y + 0.54f, pathfinder.path[pathfinder.path.Count - 1].transform.position.z)))
        {
            Vector3.MoveTowards(transform.position, new Vector3(pathfinder.path[nextNode].transform.position.x, pathfinder.path[nextNode].transform.position.y + 0.54f, pathfinder.path[nextNode].transform.position.z), 0.25f);
            if (transform.position.Equals(new Vector3(pathfinder.path[nextNode].transform.position.x, pathfinder.path[nextNode].transform.position.y + 0.54f, pathfinder.path[nextNode].transform.position.z)))
            {
                if (nextNode <= pathfinder.path.Count)
                {
                    nextNode++;
                }
            }
        }
        if (transform.position.Equals(new Vector3(pathfinder.path[pathfinder.path.Count - 1].transform.position.x, pathfinder.path[pathfinder.path.Count - 1].transform.position.y + 0.54f, pathfinder.path[pathfinder.path.Count - 1].transform.position.z)))
        {
            moveComplete = true;
            nextNode = 0;
        }
        //TURN TO FACE TARGET
    }

    public void PositionBonus()
    {
        positionBonus.Clear();
        Vector3 forward = transform.forward;
        /*foreach (KeyValuePair<Node, int> node in mapGraph.mapNodeDict)
        {
            float x = Vector3.forward.x - node.Key.transform.position.x;
            float z = Vector3.right.z - node.Key.transform.position.z;
            if (x >= 0)
            {
                positionBonus.Add(node.Key, 0.85f);
            }
            if (x < 1 && x > -1)
            {
                positionBonus.Add(node.Key, 1.1f);
            }
            if (x <= -1)
            {
                positionBonus.Add(node.Key, 1.4f);
            }
        }*/
    }

}
