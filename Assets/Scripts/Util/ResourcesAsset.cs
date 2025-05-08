using Newtonsoft.Json;
using UnityEngine;

public abstract class ResourcesAsset : ScriptableObject
{
    [JsonProperty][Unwitable] public string filename;

    [DeserializationFactory]
    public static ScriptableObject Load(string filename)
    {
        return Resources.Load<ScriptableObject>(filename);
    }
}