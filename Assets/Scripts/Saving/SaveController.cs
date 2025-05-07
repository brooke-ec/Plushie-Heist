
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using TNRD;
using UnityEngine;

public class SaveController : MonoBehaviour, ISavableMap
{
    public static readonly JsonSerializer serializer = new JsonSerializer();

    static SaveController()
    {
        serializer.Converters.Add(FactoryConverter.instance);
        serializer.ContractResolver = ContractResolver.instance;
    }

    [SerializeField] private SerializableInterface<ISavable>[] nodes;

    public static string slot = "default";
    private string path => Application.persistentDataPath + "/" + slot + ".json";

    string ISavable.key => throw new System.NotImplementedException();

    private void Start()
    {
        //Load();
    }

    public void Load()
    {
        using StreamReader sr = new StreamReader(path);
        using JsonReader jr = new JsonTextReader(sr);

        serializer.Deserialize(jr, typeof(FurnitureController));
    }

    public void Save()
    {
        using StreamWriter sw = new StreamWriter(path);
        using JsonWriter jw = new JsonTextWriter(sw);

        serializer.Serialize(jw, this.Serialize());

        Debug.Log($"Data succesfully saved to '{path}'");
    }

    ISavable[] ISavableMap.Collect()
    {
        return nodes.Select(i => i.Value).ToArray();
    }
}
