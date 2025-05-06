using Newtonsoft.Json.Linq;
using System.Linq;

public interface ISavableArray : ISavable
{
    void ISavable.Deserialize(JObject obj)
    {

    }

    JToken ISavable.Serialize()
    {
        return new JArray(Collect().Select(s => s.Serialize()));
    }

    ISavable[] Collect();
}
