using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;

namespace Runner.Agent.Version.Vers
{
    public static class VersionInfo
    {
        private static string FilePath()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vers", "versionsinfo.json");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            return path;
        }

        private static JsonNode Read()
        {
            var path = FilePath();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            using (var streamReader = File.OpenRead(path))
            {
                var doc = JsonSerializer.Deserialize<JsonNode>(streamReader, options);
                if (doc is null)
                {
                    throw new JsonException("Invalid versionsinfo.json content!");
                }
                return doc;
            }
        }

        private static void Write(JsonNode json)
        {
            var path = FilePath();

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };

            var jsonWriterOptions = new JsonWriterOptions
            {
                Indented = true
            };

            using (var streamWrite = File.OpenWrite(path))
            using (var jsonWriter = new Utf8JsonWriter(streamWrite, jsonWriterOptions))
            {
                JsonSerializer.Serialize(jsonWriter, json, jsonSerializerOptions);
                streamWrite.SetLength(jsonWriter.BytesCommitted);
            }
        }

        public static string ReadVersionActual()
        {
            var json = Read();

            var versionActualProp = json["versionActual"];
            if (versionActualProp is null)
            {
                throw new JsonException("Invalid versionsinfo.json, versionActual null!");
            }

            var versionActual = versionActualProp.GetValue<int>();

            var versionsProp = json["versions"];
            if (versionsProp is null)
            {
                throw new JsonException("Invalid versionsinfo.json, versionActual null!");
            }

            var versions = versionsProp.AsArray()
                .Select(p => p?.GetValue<string>())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToArray();

            if (versionActual < versions.Length)
            {
                return versions[versionActual]!;
            }
            else
            {
                throw new Exception("Invalid versionActual");
            }
        }

        public static void PeformADownGrade()
        {
            var json = Read();

            var versionActualProp = json["versionActual"];
            if (versionActualProp is null)
            {
                throw new JsonException("Invalid versionsinfo.json, versionActual null!");
            }

            var versionActual = versionActualProp.GetValue<int>();

            if (versionActual > 0)
            {
                json["versionActual"] = versionActual - 1;
                Write(json);
            }
        }

        public static void PerformUpgrade(string versionName)
        {
            var json = Read();

            var versionActualProp = json["versionActual"];
            if (versionActualProp is null)
            {
                throw new JsonException("Invalid versionsinfo.json, versionActual null!");
            }

            var versionActual = versionActualProp.GetValue<int>();

            var versionsProp = json["versions"];
            if (versionsProp is null)
            {
                throw new JsonException("Invalid versionsinfo.json, versionActual null!");
            }

            var versions = versionsProp.AsArray()
                .Select(p => p?.GetValue<string>())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToList();

            var has = versions.IndexOf(versionName);
            if (has > -1)
            {
                versions.RemoveAt(has);
            }

            versions.Add(versionName);
            versionActual = versions.Count - 1;

            json["versionActual"] = versionActual;
            versionsProp.ReplaceWith(versions);
            Write(json);
        }
    }
}
