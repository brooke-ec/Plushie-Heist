using UnityEngine;

[CreateAssetMenu(fileName = "New Prop List", menuName ="Scriptable Objects/Prop List", order = 9999)]
public class SpawnablePropList : ScriptableObject
{
    public GameObject[] Props;
}
