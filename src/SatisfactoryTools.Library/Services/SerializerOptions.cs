namespace SatisfactoryTools.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;

    internal class SerializerOptionsFactory
    {
        public static JsonSerializerOptions CreateSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                IgnoreReadOnlyProperties = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Skip,
                IgnoreNullValues = true
            };
        }
    }
}