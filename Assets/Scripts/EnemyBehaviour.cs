using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Pathfinder pathfinder;

    public List<ActionPriority> actionPriorityList = new List<ActionPriority>();

    public struct ActionPriority
    {
        public ActionBase action;
        public Node node;
        public GameObject target;
        public float priority;
        public bool moveThenAct;
    }

    private void Awake()
    {
        pathfinder = gameObject.GetComponent<Pathfinder>();
    }

    public void GetActionPriority()
    {
        foreach (ActionBase action in gameObject.GetComponent<EnemyController>().baseCharacter.actionPriority)
        {
            Debug.Log("Action usable: " + action.IsUsable());
            if (action.IsUsable())
            {
                Debug.Log("Action type: " + action.actionType);
                if (action.actionType == "Attacking")
                {
                    foreach (KeyValuePair<Node, float> node in pathfinder.dijkstraDistDict)
                    {
                        bool tempMoveThenAct = true;
                        if (node.Key == pathfinder.currentNode)
                        {
                            tempMoveThenAct = false;
                        }
                        if (node.Key != pathfinder.currentNode)
                        {
                            tempMoveThenAct = true;
                        }
                        foreach (GameObject character in GameObject.FindGameObjectsWithTag(action.targetType))
                        {
                            float tempPriority = 0;
                            float distNodeToPlayerCharacter = Mathf.Abs(character.transform.position.x - node.Key.transform.position.x) + Mathf.Abs(character.transform.position.z - node.Key.transform.position.z);
                            float levelDifferenceNodeToPlayerCharacter =  character.GetComponent<Pathfinder>().currentNode.nodeLevel - node.Key.nodeLevel;
                           
                            Debug.Log("Level difference node to player character: " + levelDifferenceNodeToPlayerCharacter);
                            if (action.range >= distNodeToPlayerCharacter)
                            {
                                if (action.positiveHeightRange >= levelDifferenceNodeToPlayerCharacter && action.negativeHeightRange <= levelDifferenceNodeToPlayerCharacter)
                                {
                                    BaseCharacter baseCharacter;
                                    if (action.actionType == "Enemy Character")
                                    {
                                        baseCharacter = character.GetComponent<EnemyController>().baseCharacter;
                                    }
                                    else
                                    {
                                        baseCharacter = character.GetComponent<PlayerCharacterController>().baseCharacter;
                                    }
                                    tempPriority += baseCharacter.availableActionList.IndexOf(action) + 1;
                                    tempPriority += GetComponent<EnemyController>().characterPriorityDict[baseCharacter];
                                    float tempMultiplier = 1;
                                    foreach (string advantage in action.advantages)
                                    {
                                        if (baseCharacter.weaknessDict.ContainsKey(advantage))
                                        {
                                            tempMultiplier += baseCharacter.weaknessDict[advantage];
                                        }
                                    }
                                    if (action.actionDamageStat == "HP")
                                    {
                                        if (baseCharacter.currentHP <= action.damage * tempMultiplier)
                                        {
                                            tempPriority += action.damage * tempMultiplier * 2;
                                        }
                                        else
                                        {
                                            tempPriority += action.damage * tempMultiplier;
                                        }
                                    }
                                    if (action.actionDamageStat == "MP")
                                    {
                                        if (baseCharacter.currentMP <= action.damage * tempMultiplier)
                                        {
                                            tempPriority += action.damage * tempMultiplier * 2;
                                        }
                                        else
                                        {
                                            tempPriority += action.damage * tempMultiplier;
                                        }
                                    }
                                    if (action.actionDamageStat == "AP")
                                    {
                                        if (baseCharacter.currentAP <= action.damage * tempMultiplier)
                                        {
                                            tempPriority += action.damage * tempMultiplier * 2;
                                        }
                                        else
                                        {
                                            tempPriority += action.damage * tempMultiplier;
                                        }
                                    }

                                    foreach (string ailment in action.ailmentsAdded.Keys)
                                    {
                                        if (!baseCharacter.ailments.Contains(ailment))
                                        {
                                            tempPriority += 1;
                                        }
                                        if (baseCharacter.weaknessDict.ContainsKey(ailment))
                                        {
                                            tempPriority += 1;
                                        }
                                    }

                                    foreach (string ailment in action.ailmentsRemoved.Keys)
                                    {
                                        if (baseCharacter.ailments.Contains(ailment))
                                        {
                                            tempPriority += 1;
                                        }
                                        if (baseCharacter.weaknessDict.ContainsKey(ailment))
                                        {
                                            tempPriority += 1;
                                        }
                                    }

                                    tempPriority += action.AccuracyModifer(node.Key, character.GetComponent<Pathfinder>().currentNode, character);

                                    tempPriority += distNodeToPlayerCharacter / 2;

                                    actionPriorityList.Add(new ActionPriority { action = action, node = node.Key, target = character, priority = tempPriority, moveThenAct = tempMoveThenAct });
                                    Debug.Log("Action added");
                                }
                            }
                        }
                    }
                }
                if (action.actionType == "Supporting")
                {
                    foreach (KeyValuePair<Node, float> node in pathfinder.dijkstraDistDict)
                    {
                        bool tempMoveThenAct = true;
                        if (node.Key == pathfinder.currentNode)
                        {
                            tempMoveThenAct = false;
                        }
                        if (node.Key != pathfinder.currentNode)
                        {
                            tempMoveThenAct = true;
                        }
                        foreach (GameObject character in GameObject.FindGameObjectsWithTag(action.targetType))
                        {
                            float tempPriority = 0;
                            float distNodeToPlayerCharacter = Mathf.Abs(character.transform.position.x - node.Key.transform.position.x) + Mathf.Abs(character.transform.position.z - node.Key.transform.position.z);
                            float levelDifferenceNodeToPlayerCharacter = node.Key.nodeLevel - character.GetComponent<Pathfinder>().currentNode.nodeLevel;
                            if (action.range >= distNodeToPlayerCharacter)
                            {
                                if (action.positiveHeightRange >= levelDifferenceNodeToPlayerCharacter && action.negativeHeightRange <= levelDifferenceNodeToPlayerCharacter)
                                {
                                    BaseCharacter baseCharacter = character.GetComponent<BaseCharacter>();
                                    tempPriority += baseCharacter.availableActionList.IndexOf(action) + 1;
                                    tempPriority += GetComponent<EnemyController>().characterPriorityDict[baseCharacter];
                                    float tempMultiplier = 1;
                                    foreach (string advantage in action.advantages)
                                    {
                                        if (baseCharacter.weaknessDict.ContainsKey(advantage))
                                        {
                                            tempMultiplier += baseCharacter.weaknessDict[advantage];
                                        }
                                    }
                                    if (action.actionDamageStat == "HP" && baseCharacter.currentHP != baseCharacter.maxHP)
                                    {
                                        float HPDifference = baseCharacter.maxHP - baseCharacter.currentHP;

                                        if (action.damage * tempMultiplier > HPDifference)
                                        {
                                            tempPriority += HPDifference;
                                        }
                                        else
                                        {
                                            tempPriority += action.damage * tempMultiplier;
                                        }
                                    }
                                    if (action.actionDamageStat == "MP" && baseCharacter.currentMP != baseCharacter.maxMP)
                                    {
                                        float MPDifference = baseCharacter.maxMP - baseCharacter.currentMP;

                                        if (action.damage * tempMultiplier > MPDifference)
                                        {
                                            tempPriority += MPDifference;
                                        }
                                        else
                                        {
                                            tempPriority += action.damage * tempMultiplier;
                                        }
                                    }
                                    if (action.actionDamageStat == "AP" && baseCharacter.currentAP != baseCharacter.maxAP)
                                    {
                                        float APDifference = baseCharacter.maxAP - baseCharacter.currentAP;

                                        if (action.damage * tempMultiplier > APDifference)
                                        {
                                            tempPriority += APDifference;
                                        }
                                        else
                                        {
                                            tempPriority += action.damage * tempMultiplier;
                                        }
                                    }

                                    foreach (string ailment in action.ailmentsAdded.Keys)
                                    {
                                        if (!baseCharacter.ailments.Contains(ailment))
                                        {
                                            tempPriority += 1;
                                        }
                                        if (baseCharacter.weaknessDict.ContainsKey(ailment))
                                        {
                                            tempPriority += 1;
                                        }
                                    }

                                    foreach (string ailment in action.ailmentsRemoved.Keys)
                                    {
                                        if (baseCharacter.ailments.Contains(ailment))
                                        {
                                            tempPriority += 1;
                                        }
                                        if (baseCharacter.weaknessDict.ContainsKey(ailment))
                                        {
                                            tempPriority += 1;
                                        }
                                    }

                                    tempPriority += action.AccuracyModifer(node.Key, character.GetComponent<Pathfinder>().currentNode, character);

                                    tempPriority += distNodeToPlayerCharacter / 2;

                                    actionPriorityList.Add(new ActionPriority { action = action, node = node.Key, target = character, priority = tempPriority, moveThenAct = tempMoveThenAct });
                                }
                            }
                        }
                    }
                }
                if (action.actionType == "Self")
                {
                    float tempPriority = 0;
                    tempPriority += action.PriorityModifier();
                }
                if (action.actionType == "Area")
                {
                    float tempPriority = 0;
                    tempPriority += action.PriorityModifier();
                }
            }
        }
        actionPriorityList.Sort((a, b) => a.priority.CompareTo(b.priority));
    }
}
