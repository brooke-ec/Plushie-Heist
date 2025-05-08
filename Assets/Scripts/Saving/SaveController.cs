
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    #region Static
    public static readonly JsonSerializer serializer = new JsonSerializer();

    static SaveController()
    {
        serializer.ContractResolver = ContractResolver.instance;

        serializer.Converters.Add(DeserializationFactoryConverter.instance);
    }
    #endregion

    [SerializeField] [JsonProperty("shop")] private ShopManager shop;

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
    }

    public void Save()
    {
        using StreamWriter sw = new StreamWriter(path);
        using JsonWriter jw = new JsonTextWriter(sw);

        serializer.Serialize(jw, this);

        Debug.Log($"Data succesfully saved to '{path}'");
    }
}
