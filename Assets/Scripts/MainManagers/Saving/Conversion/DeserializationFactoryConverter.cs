using System.Reflection;
using System;
using Newtonsoft.Json;
using System.Linq;
using Unity.VisualScripting;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class DeserializationFactoryConverter : JsonConverter
{
    public static readonly DeserializationFactoryConverter instance = new DeserializationFactoryConverter();

    public override bool CanWrite => false;

    public static bool TryGetFactory(Type type, out MethodInfo method)
    {
        MethodInfo[] constructors = GetMethods(type, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.HasAttribute<DeserializationFactoryAttribute>()).ToArray();

        if (constructors.Length > 1) UnityEngine.Debug.LogWarning($"'{type.Name}' has defined multiple deserialization factories, picking arbitrarily");
        else if (constructors.Length == 0)
        {
            method = null;
            return false;
        }

        method = constructors[0];
        if (method.ReturnType.IsAssignableFrom(type)) return true;
        UnityEngine.Debug.LogWarning($"'{type.Name}' defines a deserialization factory that doesn't return itself, using default deserializer");
        return false;
    }

    public override bool CanConvert(Type objectType)
    {
        return TryGetFactory(objectType, out MethodInfo _);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        object instance;
        if (!TryGetFactory(objectType, out MethodInfo method)) throw new UnreachableException();
        // Create JObject for manual manipulation
        JToken jt = JToken.Load(reader);

        if (jt.Type == JTokenType.Null) return null;
        else if (jt is JObject jo)
        {
            // Match function parameters
            object[] parameters = method.GetParameters().Select(p =>
                {
                    if (jo.TryGetValue(p.Name, out JToken t))
                    {
                        object value = t.ToObject(p.ParameterType, serializer);
                        if (value != null) return value;
                    }

                    throw new ArgumentException($"Could deserialize parameter '{p.Name}' in '{objectType.Name}' deserialization factory");
                }).ToArray();

            // Run factory and populate fields
            instance = method.Invoke(null, parameters);
            serializer.Populate(jo.CreateReader(), instance);
            return instance;
        } else throw new JsonReaderException("Current JsonReader item is not an object: " + jt.GetType().Name);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public static MethodInfo[] GetMethods(Type type, BindingFlags flags)
    {
        List<MethodInfo> methods = new List<MethodInfo>();
        for (; type != null; type = type.BaseType) methods.AddRange(type.GetMethods(flags));
        return methods.ToArray();
    }
}