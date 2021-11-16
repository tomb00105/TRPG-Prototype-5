using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    public BaseCharacter baseCharacter;
    public Pathfinder pathfinder;
    public Dictionary<string, bool> currentState = new Dictionary<string, bool>();
    public Dictionary<Node, float> positionBonus = new Dictionary<Node, float>();

    private void Awake()
    {
        pathfinder = GetComponent<Pathfinder>();
    }

    public void TurnStart()
    {

    }
}
