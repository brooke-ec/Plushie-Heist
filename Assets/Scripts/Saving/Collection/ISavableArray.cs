using Newtonsoft.Json.Linq;
using System.Linq;

public interface ISavableArray : ISavable
{
    void ISavable.Deserialize(JToken token)
    {

    }

    JToken ISavable.Serialize()
    {
        return new JArray(Collect().Select(s => s.Serialize()));
    }

    ISavable[] Collect();
}
