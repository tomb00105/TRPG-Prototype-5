using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters", order = 1)]
public class BaseCharacter : ScriptableObject
{
    public string characterSide;

    public string characterName;
    public int maxHP;
    public int currentHP;
    public int maxMP;
    public int currentMP;
    public int maxAP;
    public int currentAP;
    public int moves;
    public int speed;

    public ClassBase currentClass;
    public Dictionary<ClassBase, int> classLevelDict = new Dictionary<ClassBase, int>();
    public List<ActionBase> actionPriority = new List<ActionBase>();
    public Dictionary<string, float> weaknessDict = new Dictionary<string, float>();
    public List<string> ailments = new List<string>();
    public List<string> forbiddenAilments = new List<string>();
    public List<ActionBase> availableActionList = new List<ActionBase>();
}
