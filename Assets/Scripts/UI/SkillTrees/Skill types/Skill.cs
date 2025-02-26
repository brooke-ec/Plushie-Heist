using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New skill", menuName = "Scriptable Objects/Skill tree/Skills/New skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public List<Skill> requirements;

    public int cost;
    public string description;

    //Won't be here in the future, just for testing purposes
    public bool branchIsUnlocked = true;

    public virtual void Unlock()
    {
        Debug.LogWarning("Nothing inside unlock");
    }
}
