using Newtonsoft.Json.Linq;

public interface ISavable
{
    string key { get; }

    void Deserialize(JToken token)
    {
        SaveController.serializer.Populate(token.CreateReader(), this);
    }

    JToken Serialize()
    {
        return JToken.FromObject(this, SaveController.serializer);
    }
}

public static class SavableExtension
{
    public static JToken Serialize(this ISavable savable)
    {
        return savable.Serialize();
    }
}
