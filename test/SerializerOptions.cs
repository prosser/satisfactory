namespace SatisfactoryTools.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;

    public static class SerializerOptions
    {
        public static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
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