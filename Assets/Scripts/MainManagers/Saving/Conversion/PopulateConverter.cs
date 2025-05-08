using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PopulateConverter : JsonConverter
{
    public bool populate { get; private set; }

    public PopulateConverter(bool populate) => this.populate = populate;

    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType)
    {
        return objectType.GetInterface(nameof(IEnumerable<object>)) != null;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (existingValue == null) throw new ArgumentNullException($"Could not populate '{reader.Path}' as it has no value");

        IEnumerable<object> result;
        if (populate) result = PopulateEnumerable(reader, objectType.GetElementType(), (IEnumerable<object>) existingValue, serializer);
        else result = (IEnumerable<object>) serializer.Deserialize(reader, objectType);

        return result;
    }

    public IEnumerable<object> PopulateEnumerable(JsonReader reader, Type arrayType, IEnumerable<object> existingValue, JsonSerializer serializer)
    {
        string path = reader.Path;
        JArray ja = JArray.Load(reader);

        int index = 0;
        foreach (object item in existingValue)
        {
            if (ja.Count <= index) throw new ArgumentException($"Could not populate '{path}' as JSON array is smaller than existing collection");
            serializer.Populate(ja[index].CreateReader(), item);
            index++; 
        }

        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}