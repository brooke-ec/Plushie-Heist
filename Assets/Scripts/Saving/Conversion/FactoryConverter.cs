using System.Reflection;
using System;
using Newtonsoft.Json;
using System.Linq;
using Unity.VisualScripting;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

public class FactoryConverter : JsonConverter
{
    public static readonly FactoryConverter instance = new FactoryConverter();

    private bool TryGetConstructor(Type type, out MethodInfo method)
    {
        MethodInfo[] constructors = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.HasAttribute<DeserializationFactoryAttribute>()).ToArray();

        if (constructors.Length > 1) UnityEngine.Debug.LogWarning($"'{type.Name}' has defined multiple load factories, picking arbitrarily");
        else if (constructors.Length == 0)
        {
            method = null;
            return false;
        }

        method = constructors[0];
        if (method.ReturnType == type) return true;
        UnityEngine.Debug.LogWarning($"'{type.Name}' defines a load that doesn't return itself, using default deserializer");
        return false;
    }

    public override bool CanConvert(Type objectType)
    {
        return TryGetConstructor(objectType, out MethodInfo _);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (!TryGetConstructor(objectType, out MethodInfo method)) throw new UnreachableException();

        // Create JObject for manual manipulation
        JObject jo = JObject.Load(reader);
        
        // Match function parameters
        object[] parameters = method.GetParameters().Select(p =>
        {
            if (jo.TryGetValue(p.Name, out JToken t))
            {
                object value = t.ToObject(p.ParameterType, serializer);
                if (value != null) return value;
            }

            throw new ArgumentException($"Could deserialize parameter '{p.Name}' in '{objectType.Name}' load factory");
        }).ToArray();
        
        // Run factory and populate fields
        object instance = method.Invoke(null, parameters);
        serializer.Populate(jo.CreateReader(), instance);
        return instance;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}