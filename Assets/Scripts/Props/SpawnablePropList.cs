using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnablePropList", menuName ="Scriptable Objects/Props/PropList")]
public class SpawnablePropList : ScriptableObject
{
    public GameObject[] Props;
}
