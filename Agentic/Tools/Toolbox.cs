using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Agentic.Tools
{
    public class Toolbox
    {
        private readonly ITool[] _tools;

        public string ChatEndString => "---END---";

        public static Toolbox Empty => new Toolbox();

        public Toolbox(params ITool[] tools)
        {
            _tools = tools;
        }

        public IList<ITool> ParseTools(string message)
        {
            var parsedTools = new List<ITool>();
            var validJsonElements = ExtractAndValidateJsonStrings(message);

            foreach (var tool in _tools)
            {
                var elements = validJsonElements.Where(e => e.TryGetProperty(nameof(ITool.Tool), out _));
                if (!elements.Any()) continue;

                var element = elements.First();
                var toolName = element.GetProperty(nameof(ITool.Tool)).GetString();
                if (tool.Tool != toolName) continue;

                // Process each property of the tool
                foreach (var property in tool.GetType().GetProperties())
                {
                    if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(ToolParameter<>))
                        continue;

                    var parameterType = property.PropertyType.GetGenericArguments()[0];
                    if (element.TryGetProperty(property.Name, out JsonElement parameterElement))
                    {
                        // Deserialize parameter value from JSON
                        dynamic parameterValue = JsonSerializer.Deserialize(parameterElement.GetRawText(), parameterType);
                        dynamic toolParameterInstance = Activator.CreateInstance(property.PropertyType);
                        toolParameterInstance.Value = parameterValue;
                        property.SetValue(tool, toolParameterInstance);
                    }
                    else
                    {
                        // Set default value for properties not included in the JSON
                        dynamic toolParameterInstance = Activator.CreateInstance(property.PropertyType);
                        dynamic defaultValue = parameterType.IsValueType ? Activator.CreateInstance(parameterType) : null;
                        toolParameterInstance.Value = defaultValue;
                        property.SetValue(tool, toolParameterInstance);
                    }
                }
                parsedTools.Add(tool);
            }

            return parsedTools;
        }

        public string CreateDefaultSystemMessage(string systemMessage)
        {
            if (_tools == null || !_tools.Any()) return systemMessage;

            var endingChatMessage = $"When you have answered ALL questions and finished ALL your current tasks successfully as asked for, or require my input, you MUST say '{ChatEndString}' to let me know that you are done.";

            var toolMessage = GetToolJsonExample();
            string toolString = _tools.Length == 1 ? "tool" : "tools";

            var toolSystemMessage = $"If you need to use a tool, don't say anything else, only use the tool. You MUST use your {toolString} to overcome any of your limitations, find information needed and perform actions required to complete your task. You have access to the following {toolString} which you invoke using this JSON schema:{Environment.NewLine}{toolMessage}";

            var finalSystemMessage = string.Join(Environment.NewLine, systemMessage, endingChatMessage, toolSystemMessage);

            return finalSystemMessage;
        }

        public string GetToolJsonExample()
        {
            if (_tools == null || !_tools.Any()) return string.Empty;

            var toolMessage = string.Join(Environment.NewLine, _tools.Select(GetToolJsonExample));

            return toolMessage;
        }

        public string GetToolJsonExample(ITool tool)
        {
            var toolType = tool.GetType();
            var toolInfo = new Dictionary<string, object>
            {
                [nameof(tool.Tool)] = tool.Tool
            };

            var properties = toolType.GetProperties()
                .Where(prop => prop.PropertyType.IsGenericType &&
                                prop.PropertyType.GetGenericTypeDefinition() == typeof(ToolParameter<>));

            foreach (var property in properties)
            {
                var parameterValue = property.GetValue(tool);
                var valueType = property.PropertyType.GetGenericArguments()[0];
                if (parameterValue != null)
                {
                    var valueProperty = parameterValue.GetType().GetProperty("Value");
                    var value = valueProperty?.GetValue(parameterValue);
                    toolInfo[property.Name] = value ?? CreateDefault(valueType);
                }
                else
                {
                    if (valueType == typeof(string))
                    {
                        toolInfo[property.Name] = $"<{property.Name}>";
                    }
                    else
                    {
                        toolInfo[property.Name] = CreateDefault(valueType);
                    }
                }
            }

            var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            var toolJson = JsonSerializer.Serialize(toolInfo, options);
            var toolExample = string.IsNullOrWhiteSpace(tool.Description) ? toolJson : $"{toolJson} ({tool.Description})";

            return toolExample;
        }

        public string GetToolInvocationMessage(ITool tool)
        {
            if (tool == null) throw new ArgumentNullException(nameof(tool));
            return GetToolJson(tool);
        }

        public static string GetToolJson(ITool tool)
        {
            var toolType = tool.GetType();
            var toolProperties = new Dictionary<string, object>
            {
                [nameof(tool.Tool)] = tool.Tool
            };

            var properties = toolType.GetProperties()
                .Where(p => p.PropertyType.IsGenericType &&
                            p.PropertyType.GetGenericTypeDefinition() == typeof(ToolParameter<>));

            foreach (var property in properties)
            {
                // Get the ToolParameter<T> instance from the tool object
                var toolParameterInstance = property.GetValue(tool);
                object value = null;

                if (toolParameterInstance != null)
                {
                    // Get the Value property from the ToolParameter<T> type
                    var valueProperty = property.PropertyType.GetProperty("Value");
                    if (valueProperty != null)
                    {
                        // Retrieve the value of the Value property from the ToolParameter<T> instance
                        value = valueProperty.GetValue(toolParameterInstance);
                    }
                }

                var valueType = property.PropertyType.GetGenericArguments()[0];
                toolProperties[property.Name] = value ?? CreateDefault(valueType);
            }

            var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            var json = JsonSerializer.Serialize(toolProperties).Replace("\\u0022", "\\\"");
            return json;
        }

        private static object CreateDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else if (type == typeof(string))
            {
                return "";
            }
            else
            {
                // Handling complex object creation and initialization
                var instance = Activator.CreateInstance(type);
                foreach (var prop in type.GetProperties().Where(p => p.CanWrite))
                {
                    var propDefaultValue = CreateDefault(prop.PropertyType);
                    prop.SetValue(instance, propDefaultValue);
                }
                return instance;
            }
        }

        private List<JsonElement> ExtractAndValidateJsonStrings(string input)
        {
            var validJsonElements = new List<JsonElement>();
            int braceCount = 0;
            StringBuilder currentJson = new StringBuilder();
            bool insideString = false;
            char lastChar = '\0';

            foreach (char c in input)
            {
                // Toggle insideString flag if we encounter a double-quote that is not escaped
                if (c == '"' && lastChar != '\\')
                {
                    insideString = !insideString;
                }

                if (!insideString)
                {
                    if (c == '{')
                    {
                        braceCount++;
                        if (braceCount == 1)
                        {
                            currentJson.Clear();
                        }
                    }
                    else if (c == '}')
                    {
                        braceCount--;
                        if (braceCount == 0)
                        {
                            currentJson.Append(c);
                            try
                            {
                                // Attempt to validate and deserialize the JSON string
                                var jsonElement = JsonSerializer.Deserialize<JsonElement>(currentJson.ToString());
                                validJsonElements.Add(jsonElement);
                            }
                            catch (JsonException)
                            {
                                // If deserialization fails, the JSON is not valid, so it's ignored
                            }
                            continue;
                        }
                    }
                }

                if (braceCount > 0)
                {
                    currentJson.Append(c);
                }

                lastChar = c; // Keep track of the last character for escaped quote handling
            }

            return validJsonElements;
        }
    }
}
