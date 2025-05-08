using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-60)]
public class SaveManager : MonoBehaviour
{
    #region Static
    public static readonly JsonSerializer serializer = new JsonSerializer();
    public static readonly UnityEvent onLoaded = new UnityEvent();
    public static bool deserializing { get; private set; }
    public static string slot = "default";

    static SaveManager()
    {
        serializer.ContractResolver = ContractResolver.instance;

        serializer.Converters.Add(DeserializationFactoryConverter.instance);
    }
    #endregion

    [JsonProperty("player", Order = -1)] internal SharedUIManager player => SharedUIManager.instance;
    [JsonProperty("shop")] internal ShopManager shop => ShopManager.instance;

    private string path => Application.persistentDataPath + "/" + slot + ".json";

    private void Start()
    {
        Load();
    }

    public void Load()
    {
        deserializing = true;
        using StreamReader sr = new StreamReader(path);
        using JsonReader jr = new JsonTextReader(sr);

        serializer.Populate(jr, this);
        onLoaded.Invoke();
        deserializing = false;
    }

    public void Save()
    {
        using StreamWriter sw = new StreamWriter(path);
        using JsonWriter jw = new JsonTextWriter(sw);

        serializer.Serialize(jw, this);

        Debug.Log($"Data succesfully saved to '{path}'");
    }
}
