using Newtonsoft.Json.Linq;

public interface ISavable
{
    string key { get; }
    void Deserialize(JObject obj);
    JToken Serialize();
}

public static class SavableExtension
{
    public static JToken Serialize(this ISavable savable)
    {
        return savable.Serialize();
    }
}
