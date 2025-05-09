using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-80)]
public class SaveManager : MonoBehaviour
{
    #region Static
    public static readonly JsonSerializer serializer = new JsonSerializer();
    public static readonly UnityEvent onLoaded = new UnityEvent();
    public static SaveManager instance { get; private set; }
    public static bool deserializing { get; private set; }
    public static string slot = "default";

    static SaveManager()
    {
        serializer.ContractResolver = ContractResolver.instance;

        serializer.Converters.Add(DeserializationFactoryConverter.instance);
    }
    #endregion

    [SerializeField] private bool loadOnStart = true;
    [JsonProperty("player", Order = -1)] internal SharedUIManager player => SharedUIManager.instance;
    [JsonProperty("shop")] internal ShopManager shop => ShopManager.instance;

    private string path => Application.persistentDataPath + "/" + slot + ".json";
    private JObject deserialized;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance);
    }

    private void Start()
    {
        if (loadOnStart) Load();
        print(shop);
    }

    public void Load()
    {
        deserializing = true;
        using StreamReader sr = new StreamReader(path);
        using JsonReader jr = new JsonTextReader(sr);

        deserialized = JObject.Load(jr); // Deserialize for merging later
        serializer.Populate(deserialized.CreateReader(), this); // Deserialize into level
        
        onLoaded.Invoke();
        deserializing = false;
    }

    public void Save()
    {
        using StreamWriter sw = new StreamWriter(path);
        using JsonWriter jw = new JsonTextWriter(sw);

        JObject serialized = JObject.FromObject(this, serializer);
        if (shop == null) serialized["shop"] = deserialized["shop"];
        serialized.WriteTo(jw);

        Debug.Log($"Data succesfully saved to '{path}'");
    }
}
