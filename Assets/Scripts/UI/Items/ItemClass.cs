using UnityEngine;

[CreateAssetMenu(fileName = "New item", menuName = "Scriptable Objects/Items/New item")]
public class ItemClass : ScriptableObject
{
    /// <summary> how many tiles it occupies (width) </summary>
    public int sizeWidth = 1;
    /// <summary> how many tiles it occupies (height) </summary>
    public int sizeHeight = 1;

    public Sprite itemIcon;
}
