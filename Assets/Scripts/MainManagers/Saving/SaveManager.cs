
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

    static SaveManager()
    {
        serializer.ContractResolver = ContractResolver.instance;

        serializer.Converters.Add(DeserializationFactoryConverter.instance);
    }
    #endregion

    [SerializeField] [JsonProperty("shop")] internal ShopManager shop;
    [JsonProperty("player")] internal SharedUIManager player => SharedUIManager.instance;

    private string path => Application.persistentDataPath + "/" + slot + ".json";
    public static string slot = "default";

    private void Start()
    {
        Load();
    }

    public void Load()
    {
        using StreamReader sr = new StreamReader(path);
        using JsonReader jr = new JsonTextReader(sr);

        serializer.Populate(jr, this);
        onLoaded.Invoke();
    }

    public void Save()
    {
        using StreamWriter sw = new StreamWriter(path);
        using JsonWriter jw = new JsonTextWriter(sw);

        serializer.Serialize(jw, this);

        Debug.Log($"Data succesfully saved to '{path}'");
    }
}
