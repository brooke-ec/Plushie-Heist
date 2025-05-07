using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Reflection;

public class ContractResolver : DefaultContractResolver
{
    public static readonly ContractResolver instance = new ContractResolver();

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        List<JsonProperty> properties = new List<JsonProperty>();
        bool unityObject = typeof(UnityEngine.Object).IsAssignableFrom(type);

        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            if (!unityObject || field.HasAttribute<JsonPropertyAttribute>())
            {
                JsonProperty p = CreateProperty(field, memberSerialization);
                p.Writable = !field.HasAttribute<UnwitableAttribute>();
                p.Readable = true;
                properties.Add(p);
            }

        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            if (property.HasAttribute<JsonPropertyAttribute>())
            {
                JsonProperty p = CreateProperty(property, memberSerialization);
                p.Writable = property.CanWrite && !property.HasAttribute<UnwitableAttribute>();
                p.Readable = property.CanRead;
                properties.Add(p);
            }

        return properties;
    }
}