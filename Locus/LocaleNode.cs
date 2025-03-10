using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Locus;

public class LocaleNode
{
    internal readonly Dictionary<string, LocaleNode> _children = new();
    private string? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocaleNode"/> class.
    /// </summary>
    /// <param name="value">The value of the locale node.</param>
    public LocaleNode(string? value = null)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a <see cref="LocaleNode"/> to a string.
    /// </summary>
    /// <param name="node">The locale node to convert.</param>
    public static implicit operator string(LocaleNode node) => node.ToString();

    /// <summary>
    /// Implicitly converts a string to a <see cref="LocaleNode"/>.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    public static implicit operator LocaleNode(string value) => new(value);

    /// <summary>
    /// Gets the child <see cref="LocaleNode"/> associated with the specified key.
    /// </summary>
    /// <param name="key">The key to look up in the children dictionary.</param>
    /// <returns>The child <see cref="LocaleNode"/> if found; otherwise, null.</returns>
    public LocaleNode? this[string key] => _children.GetValueOrDefault(key);

    /// <summary>
    /// Adds a child <see cref="LocaleNode"/> to the current node.
    /// </summary>
    /// <param name="key">The key to associate with the child node.</param>
    /// <param name="node">The child node to add.</param>
    public void AddChild(string key, LocaleNode node)
    {
        _children[key] = node;
    }
    
    /// <summary>
    /// Formats the current node with the specified values.
    /// </summary>
    /// <param name="values">The object containing the values.</param>
    /// <returns>The formatted node string.</returns>
    public string Format(object values)
    {
        if (string.IsNullOrWhiteSpace(_value)) return string.Empty;

        var result = _value;
        var properties = values.GetType().GetProperties();
        
        foreach (var prop in properties)
        {
            var placeholder = $"{{{prop.Name}}}";
            result = result.Replace(placeholder, prop.GetValue(values)?.ToString() ?? string.Empty);
        }

        return result;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return _value ?? string.Empty;
    }

    /// <summary>
    /// Creates a <see cref="LocaleNode"/> from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>The created <see cref="LocaleNode"/>.</returns>
    public static LocaleNode FromJson(string json)
    {
        using (JsonDocument doc = JsonDocument.Parse(json))
        {
            var rootNode = new LocaleNode();
            AddChildrenFromJsonElement(rootNode, doc.RootElement);
            return rootNode;
        }
    }

    /// <summary>
    /// Recursively populates the current node with default values from another node.
    /// </summary>
    /// <param name="defaults">The node containing default values.</param>
    internal void RecursivePopulateWithDefaults(LocaleNode defaults)
    {
        foreach (var (key, value) in defaults._children)
        {
            if (!_children.ContainsKey(key))
            {
                _children[key] = value;
            }
            else
            {
                _children[key].RecursivePopulateWithDefaults(value);
            }
        }
    }

    /// <summary>
    /// Adds children to a parent node from a JSON element.
    /// </summary>
    /// <param name="parentNode">The parent node to add children to.</param>
    /// <param name="element">The JSON element to parse.</param>
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