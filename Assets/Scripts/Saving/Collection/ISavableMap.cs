using Newtonsoft.Json.Linq;

public interface ISavableMap : ISavable
{
    void ISavable.Deserialize(JToken token)
    {

    }

    JToken ISavable.Serialize()
    {
        JObject jo = new JObject();
        Collect().ForEach(s => jo.Add(s.key, s.Serialize()));
        return jo;
    }

    ISavable[] Collect();
}
