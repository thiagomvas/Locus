using System;
using System.Collections.Generic;
using System.Text.Json;

public class LocaleNode
{
    internal readonly Dictionary<string, LocaleNode> _children = new();
    private string? _value;
    
    public LocaleNode(string? value = null)
    {
        _value = value;
    }

    public static implicit operator string(LocaleNode node) => node.ToString();
    public static implicit operator LocaleNode(string value) => new(value);

    public LocaleNode? this[string key] => _children.GetValueOrDefault(key);

    public void AddChild(string key, LocaleNode node)
    {
        _children[key] = node;
    }

    public override string ToString()
    {
        return _value ?? string.Empty; 
    }

    public static LocaleNode FromJson(string json)
    {
        using (JsonDocument doc = JsonDocument.Parse(json))
        {
            var rootNode = new LocaleNode();
            AddChildrenFromJsonElement(rootNode, doc.RootElement);
            return rootNode;
        }
    }

    internal void RecursivePopulateWithDefaults(LocaleNode defaults)
    {
        foreach (var (key, value) in defaults._children)
        {
            if (!_children.ContainsKey(key))
            {
                _children[key] = value;
                Console.WriteLine($"Added default value for key: {key} | {JsonSerializer.Serialize(_children.Values.Select(n => n.ToString()))}");
            }
            else
            {
                _children[key].RecursivePopulateWithDefaults(value);
            }
        }
        
    }

    private static void AddChildrenFromJsonElement(LocaleNode parentNode, JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object) return;
        
        foreach (var property in element.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.Object)
            {
                var childNode = new LocaleNode();
                AddChildrenFromJsonElement(childNode, property.Value);
                parentNode.AddChild(property.Name, childNode);
            }
            else if (property.Value.ValueKind == JsonValueKind.String)
            {
                parentNode.AddChild(property.Name, new LocaleNode(property.Value.GetString()));
            }
        }
    }
}
