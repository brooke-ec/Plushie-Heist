using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New skill", menuName = "Scriptable Objects/Skill tree/Skills/New skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    public List<Skill> requirements;

    public int cost;
    public Sprite icon;
    public string description;

    public virtual void Unlock()
    {
        Debug.LogWarning("Nothing inside unlock");
    }
}
