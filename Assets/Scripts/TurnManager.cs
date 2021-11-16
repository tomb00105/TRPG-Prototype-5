using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    int turnNumber = 1;
    int internalTurnNumber = 0;

    public Dictionary<GameObject, int> turnOrderDict = new Dictionary<GameObject, int>();
    public List<GameObject> turnOrderList = new List<GameObject>();

    private void Start()
    {
        GetTurnOrder();
        if (turnOrderList[0].CompareTag("Enemy Character"))
        {
            turnOrderList[0].GetComponent<EnemyController>().TurnStart();
        }
        if (turnOrderList[0].CompareTag("Player Character"))
        {
            turnOrderList[0].GetComponent<PlayerCharacterController>().TurnStart();
        }
    }

    public void GetTurnOrder()
    {
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Enemy Character"))
        {
            turnOrderDict.Add(character, character.GetComponent<EnemyController>().baseCharacter.speed);
            turnOrderList.Add(character);
        }
        /*foreach (GameObject character in GameObject.FindGameObjectsWithTag("Player Character"))
        {
            turnOrderDict.Add(character, character.GetComponent<EnemyController>().baseCharacter.speed);
            turnOrderList.Add(character);
        }*/
        turnOrderList.Sort((a, b) => turnOrderDict[a].CompareTo(turnOrderDict[b]));
    }

    public void EndTurn()
    {
        if (turnOrderList[internalTurnNumber].GetComponent<EnemyController>().baseCharacter.characterSide == "Enemy")
        {
            turnOrderList[internalTurnNumber].GetComponent<EnemyController>().isTurn = false;
        }
        if (internalTurnNumber >= turnOrderList.Count)
        {
            internalTurnNumber = 0;
        }
        else
        {
            internalTurnNumber++;
        }
        turnNumber++;
        turnOrderList[internalTurnNumber].GetComponent<EnemyController>().TurnStart();
    }
}
