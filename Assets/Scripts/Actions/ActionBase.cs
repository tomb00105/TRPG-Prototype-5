using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Action", menuName = "Actions", order = 1)]
public class ActionBase : ScriptableObject
{
    public string actionName;
    public int damage;
    public string actionDamageStat;
    public int range;
    public int positiveHeightRange;
    public int negativeHeightRange;
    public string targetType;
    public string actionType;
    public int mPCost;
    public int aPCost;
    public int hPCost;
    public float accuracy;

    public Dictionary<float, float> rangeAccuracyModifier = new Dictionary<float, float>();
    public Dictionary<float, float> heightAccuracyModifier = new Dictionary<float, float>();

    public List<string> advantages = new List<string>();
    public Dictionary<string, float> ailmentsAdded = new Dictionary<string, float>();
    public Dictionary<string, float> ailmentsRemoved = new Dictionary<string, float>();

    public bool IsUsable()
    {
        return true;
    }

    public float AccuracyModifer(Node origin, Node targetNode, GameObject targetCharacter)
    {
        return accuracy;
    }

    public float PriorityModifier()
    {
        return 0;
    }

    public void TakeAction()
    {

    }

}
