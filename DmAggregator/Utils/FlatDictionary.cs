using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DmAggregator.Utils
{
    /// <summary>
    /// Credit: Microsoft, based on JsonConfigurationFileParser
    /// Use MS very elegant JSON configuration file parser generating a flat dictionary.
    /// Originally it parsed a JSON file stream.
    /// Ron, Added loading an object, using a custom JsonSerializer, and hiding configurable sensitive property names.
    /// </summary>
    public sealed class FlatDictionary
    {
        private static readonly JsonSerializerOptions s_defaultWebSerializerOptions;

        private readonly Dictionary<string, string> _data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private readonly Stack<string> _paths = new Stack<string>();
        private readonly int _maxDepth;
        private int _depth;

        static FlatDictionary()
        {
            s_defaultWebSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            };
            s_defaultWebSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        private FlatDictionary(int maxDepth)
        {
            this._maxDepth = maxDepth;
        }

        /// <summary>
        /// Parse stream to flat dictionary
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public static IDictionary<string, string> Parse(Stream input, int maxDepth = int.MaxValue)
        {
            return new FlatDictionary(maxDepth).ParseStream(input);
        }

        /// <summary>
        /// Parse object to flat dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public static IDictionary<string, string> FromObject<T>(T obj, int maxDepth = int.MaxValue)
        {
            var dict = new FlatDictionary(maxDepth).ParseObject(obj);
            return dict;
        }

        private IDictionary<string, string> ParseStream(Stream input)
        {
            JsonDocumentOptions jsonDocumentOptions = default(JsonDocumentOptions);
            jsonDocumentOptions.CommentHandling = JsonCommentHandling.Skip;
            jsonDocumentOptions.AllowTrailingCommas = true;
            JsonDocumentOptions options = jsonDocumentOptions;
            using (StreamReader streamReader = new StreamReader(input))
            {
                using JsonDocument jsonDocument = JsonDocument.Parse(streamReader.ReadToEnd(), options);
                if (jsonDocument.RootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new FormatException($"InvalidTopLevelJSONElement '{jsonDocument.RootElement.ValueKind}'");
                }
                VisitElement(jsonDocument.RootElement);
            }
            return _data;
        }

        private IDictionary<string, string> ParseObject<T>(T obj)
        {
            using JsonDocument jsonDocument = JsonSerializer.SerializeToDocument(obj, s_defaultWebSerializerOptions);
            VisitElement(jsonDocument.RootElement);
            return _data;
        }


        private void VisitElement(JsonElement element)
        {
            bool flag = true;
            foreach (JsonProperty item in element.EnumerateObject())
            {
                flag = false;
                EnterContext(item.Name);
                VisitValue(item.Value);
                ExitContext();
            }
            if (flag && _paths.Count > 0)
            {
                _data[_paths.Peek()] = null!;
            }
        }

        private void VisitValue(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Object:
                    if (this._depth < this._maxDepth)
                    {
                        VisitElement(value);
                    }
                    break;
                case JsonValueKind.Array:
                    {
                        int num = 0;
                        {
                            foreach (JsonElement item in value.EnumerateArray())
                            {
                                EnterContext(num.ToString());
                                VisitValue(item);
                                ExitContext();
                                num++;
                            }
                            break;
                        }
                    }
                case JsonValueKind.String:
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    {
                        string text = _paths.Peek();
                        if (_data.ContainsKey(text))
                        {
                            throw new FormatException($"duplicate key '{text}'");
                        }
                        _data[text] = value.ToString();
                        break;
                    }
                default:
                    throw new FormatException($"UnsupportedJSONToken '{value.ValueKind}'");
            }
        }

        private void EnterContext(string context)
        {
            _paths.Push((_paths.Count > 0) ? (_paths.Peek() + '.' + context) : context);
            this._depth++;
        }

        private void ExitContext()
        {
            _paths.Pop();
            this._depth--;
        }
    }
}