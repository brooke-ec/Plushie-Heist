using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;

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

                // Use collection converter if applicable
                PopulateConverter converter = new PopulateConverter(field.HasAttribute<PopulateAttribute>());
                if (converter.CanConvert(field.FieldType)) p.Converter = converter;
                properties.Add(p);
            }

        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            if (property.HasAttribute<JsonPropertyAttribute>())
            {
                JsonProperty p = CreateProperty(property, memberSerialization);
                p.Writable = property.CanWrite && !property.HasAttribute<UnwitableAttribute>();
                p.Readable = property.CanRead;

                // Use collection converter if applicable
                PopulateConverter converter = new PopulateConverter(property.HasAttribute<PopulateAttribute>());
                if (converter.CanConvert(property.PropertyType)) p.Converter = converter;
                properties.Add(p);
            }

        return properties.OrderBy(p => p.Order).ToArray();
    }
}