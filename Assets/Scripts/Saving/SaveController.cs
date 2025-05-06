
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using TNRD;
using UnityEngine;

public class SaveController : MonoBehaviour, ISavableMap
{
    [SerializeField] private SerializableInterface<ISavable>[] nodes;

    public static string slot = "default";
    private string path => Application.persistentDataPath + "/" + slot + ".json";

    string ISavable.key => throw new System.NotImplementedException();

    public void Save()
    {
        JsonSerializer json = new JsonSerializer();

        using StreamWriter sw = new StreamWriter(path);
        using JsonWriter jw = new JsonTextWriter(sw);

        json.Serialize(jw, this.Serialize());

        Debug.Log($"Data succesfully saved to '{path}'");
    }

    ISavable[] ISavableMap.Collect()
    {
        return nodes.Select(i => i.Value).ToArray();
    }
}
