using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Helper class for easy deserialization of scriptable objects
/// </summary>
public abstract class ResourcesAsset : ScriptableObject
{
    [JsonProperty][Unwitable] public string filename;

    [DeserializationFactory]
    public static ScriptableObject Load(string filename)
    {
        return Resources.Load<ScriptableObject>(filename);
    }
}