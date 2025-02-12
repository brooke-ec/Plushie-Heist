using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New skill", menuName = "Scriptable Objects/Skill tree/New skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public List<Skill> requirements;
}
