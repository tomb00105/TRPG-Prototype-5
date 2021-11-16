using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGraph : MonoBehaviour
{
    public Dictionary<Node, int> mapNodeDict = new Dictionary<Node, int>();
    public Dictionary<GameObject, Vector3> characterPosDict = new Dictionary<GameObject, Vector3>();

    private void Awake()
    {
        foreach (Node node in GameObject.FindObjectsOfType<Node>())
        {
            mapNodeDict.Add(node, node.nodeLevel);
        }
        foreach (GameObject playerCharacter in GameObject.FindGameObjectsWithTag("Player Character"))
        {
            characterPosDict.Add(playerCharacter, playerCharacter.transform.position);
        }
        foreach (GameObject enemyCharacter in GameObject.FindGameObjectsWithTag("Enemy Character"))
        {
            characterPosDict.Add(enemyCharacter, enemyCharacter.transform.position);
        }
    }

    private void SetBlockedNodes(int characterSide)
    {
        foreach (Node node in mapNodeDict.Keys)
        {
            node.isBlocked = false;
        }
        if (characterSide == 0)
        {
            foreach (GameObject enemyCharacter in GameObject.FindGameObjectsWithTag("Enemy Character"))
            {
                foreach (Node node in mapNodeDict.Keys)
                {
                    if (enemyCharacter.transform.position.x == node.transform.position.x && enemyCharacter.transform.position.z == node.transform.position.z)
                    {
                        node.isBlocked = true;
                    }
                }
            }
        }
        if (characterSide == 1)
        {
            foreach (GameObject playerCharacter in GameObject.FindGameObjectsWithTag("Player Character"))
            {
                foreach (Node node in mapNodeDict.Keys)
                {
                    if (playerCharacter.transform.position.x == node.transform.position.x && playerCharacter.transform.position.z == node.transform.position.z)
                    {
                        node.isBlocked = true;
                    }
                }
            }
        }
    }
}
