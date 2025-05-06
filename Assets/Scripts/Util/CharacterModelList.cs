using UnityEngine;

[CreateAssetMenu(fileName = "Character Model List", menuName = "Scriptable Objects/Character Model List", order = 9999)]
public class CharacterModelList : ScriptableObject
{
    public Animator[] models;
}