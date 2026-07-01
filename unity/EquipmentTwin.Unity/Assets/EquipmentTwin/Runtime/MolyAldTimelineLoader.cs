using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EquipmentTwin.Unity.Processes
{
    public static class MolyAldTimelineLoader
    {
        public const string SupportedSchemaVersion = "equipment-twin.moly-ald.timeline.v1";
        private const RegexOptions JsonRegexOptions = RegexOptions.CultureInvariant | RegexOptions.Singleline;

        public static MolyAldTimelineDocumentDto FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Timeline JSON is empty.", nameof(json));
            }

            var timeline = new MolyAldTimelineDocumentDto
            {
                schemaVersion = ExtractString(json, "schemaVersion"),
                source = ExtractString(json, "source"),
                recipeName = ExtractString(json, "recipeName"),
                success = ExtractBool(json, "success"),
                finalStep = ExtractString(json, "finalStep"),
                faultScenarioName = ExtractString(json, "faultScenarioName"),
                stationCount = ExtractInt(json, "stationCount"),
                cycleCount = ExtractInt(json, "cycleCount"),
                targetThicknessAngstrom = ExtractFloat(json, "targetThicknessAngstrom"),
                estimatedThicknessAngstrom = ExtractFloat(json, "estimatedThicknessAngstrom"),
                totalDurationMilliseconds = ExtractFloat(json, "totalDurationMilliseconds"),
                publicBasis = ExtractStringArray(json, "publicBasis"),
                steps = ExtractSteps(json)
            };

            Validate(timeline);
            return timeline;
        }

        public static MolyAldTimelineDocumentDto FromTextAsset(TextAsset timelineAsset)
        {
            if (timelineAsset == null)
            {
                throw new ArgumentNullException(nameof(timelineAsset));
            }

            return FromJson(timelineAsset.text);
        }

        public static MolyAldTimelineDocumentDto FromStreamingAssetsFile(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentException("StreamingAssets relative path is required.", nameof(relativePath));
            }

            var path = Path.Combine(Application.streamingAssetsPath, relativePath);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Timeline JSON was not found: {path}", path);
            }

            return FromJson(File.ReadAllText(path));
        }

        private static string ExtractString(string json, string name, string defaultValue = "")
        {
            var match = Regex.Match(
                json,
                $"\"{Regex.Escape(name)}\"\\s*:\\s*\"(?<value>(?:\\\\.|[^\"\\\\])*)\"",
                JsonRegexOptions);

            return match.Success
                ? Regex.Unescape(match.Groups["value"].Value)
                : defaultValue;
        }

        private static string[] ExtractStringArray(string json, string name)
        {
            var section = ExtractArraySection(json, name);
            if (string.IsNullOrWhiteSpace(section))
            {
                return new string[0];
            }

            var values = new List<string>();
            foreach (Match match in Regex.Matches(section, "\"(?<value>(?:\\\\.|[^\"\\\\])*)\"", JsonRegexOptions))
            {
                values.Add(Regex.Unescape(match.Groups["value"].Value));
            }

            return values.ToArray();
        }

        private static bool ExtractBool(string json, string name, bool defaultValue = false)
        {
            var match = Regex.Match(
                json,
                $"\"{Regex.Escape(name)}\"\\s*:\\s*(?<value>true|false)",
                JsonRegexOptions | RegexOptions.IgnoreCase);

            return match.Success
                ? string.Equals(match.Groups["value"].Value, "true", StringComparison.OrdinalIgnoreCase)
                : defaultValue;
        }

        private static int ExtractInt(string json, string name, int defaultValue = 0)
        {
            var match = Regex.Match(
                json,
                $"\"{Regex.Escape(name)}\"\\s*:\\s*(?<value>-?\\d+)",
                JsonRegexOptions);

            return match.Success && int.TryParse(match.Groups["value"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }

        private static float ExtractFloat(string json, string name, float defaultValue = 0f)
        {
            var match = Regex.Match(
                json,
                $"\"{Regex.Escape(name)}\"\\s*:\\s*(?<value>-?(?:\\d+\\.?\\d*|\\.\\d+)(?:[eE][+-]?\\d+)?)",
                JsonRegexOptions);

            return match.Success && float.TryParse(match.Groups["value"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultValue;
        }

        private static MolyAldTimelineStepDto[] ExtractSteps(string json)
        {
            var stepsSection = ExtractArraySection(json, "steps");
            if (string.IsNullOrWhiteSpace(stepsSection))
            {
                return new MolyAldTimelineStepDto[0];
            }

            var steps = new List<MolyAldTimelineStepDto>();
            foreach (var stepJson in SplitTopLevelObjects(stepsSection))
            {
                var valvesJson = ExtractObjectSection(stepJson, "valves");
                steps.Add(new MolyAldTimelineStepDto
                {
                    index = ExtractInt(stepJson, "index"),
                    step = ExtractString(stepJson, "step"),
                    cycle = ExtractInt(stepJson, "cycle"),
                    startedAtUtc = ExtractString(stepJson, "startedAtUtc"),
                    completedAtUtc = ExtractString(stepJson, "completedAtUtc"),
                    durationMilliseconds = ExtractFloat(stepJson, "durationMilliseconds"),
                    success = ExtractBool(stepJson, "success"),
                    message = ExtractString(stepJson, "message"),
                    chamberPressureMtorr = ExtractFloat(stepJson, "chamberPressureMtorr"),
                    waferTemperatureC = ExtractFloat(stepJson, "waferTemperatureC"),
                    estimatedThicknessAngstrom = ExtractFloat(stepJson, "estimatedThicknessAngstrom"),
                    valves = new MolyAldTimelineValvesDto
                    {
                        metalPrecursor = ExtractBool(valvesJson, "metalPrecursor"),
                        reactant = ExtractBool(valvesJson, "reactant"),
                        purge = ExtractBool(valvesJson, "purge")
                    }
                });
            }

            return steps.ToArray();
        }

        private static string ExtractArraySection(string json, string name)
        {
            return ExtractBracketedSection(json, name, '[', ']');
        }

        private static string ExtractObjectSection(string json, string name)
        {
            return ExtractBracketedSection(json, name, '{', '}');
        }

        private static string ExtractBracketedSection(string json, string name, char open, char close)
        {
            var keyIndex = json.IndexOf($"\"{name}\"", StringComparison.Ordinal);
            if (keyIndex < 0)
            {
                return string.Empty;
            }

            var start = json.IndexOf(open, keyIndex);
            if (start < 0)
            {
                return string.Empty;
            }

            var depth = 0;
            var inString = false;
            var escaped = false;
            for (var index = start; index < json.Length; index++)
            {
                var current = json[index];
                if (inString)
                {
                    if (escaped)
                    {
                        escaped = false;
                    }
                    else if (current == '\\')
                    {
                        escaped = true;
                    }
                    else if (current == '"')
                    {
                        inString = false;
                    }

                    continue;
                }

                if (current == '"')
                {
                    inString = true;
                }
                else if (current == open)
                {
                    depth++;
                }
                else if (current == close)
                {
                    depth--;
                    if (depth == 0)
                    {
                        return json.Substring(start + 1, index - start - 1);
                    }
                }
            }

            return string.Empty;
        }

        private static IEnumerable<string> SplitTopLevelObjects(string jsonArrayContent)
        {
            var depth = 0;
            var start = -1;
            var inString = false;
            var escaped = false;

            for (var index = 0; index < jsonArrayContent.Length; index++)
            {
                var current = jsonArrayContent[index];
                if (inString)
                {
                    if (escaped)
                    {
                        escaped = false;
                    }
                    else if (current == '\\')
                    {
                        escaped = true;
                    }
                    else if (current == '"')
                    {
                        inString = false;
                    }

                    continue;
                }

                if (current == '"')
                {
                    inString = true;
                }
                else if (current == '{')
                {
                    if (depth == 0)
                    {
                        start = index;
                    }

                    depth++;
                }
                else if (current == '}')
                {
                    depth--;
                    if (depth == 0 && start >= 0)
                    {
                        yield return jsonArrayContent.Substring(start, index - start + 1);
                        start = -1;
                    }
                }
            }
        }

        private static void Validate(MolyAldTimelineDocumentDto timeline)
        {
            if (timeline == null)
            {
                throw new InvalidOperationException("Timeline JSON could not be parsed.");
            }

            if (!string.Equals(timeline.schemaVersion, SupportedSchemaVersion, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Unsupported timeline schema '{timeline.schemaVersion}'. Expected '{SupportedSchemaVersion}'.");
            }

            if (timeline.steps == null || timeline.steps.Length == 0)
            {
                throw new InvalidOperationException("Timeline requires at least one step.");
            }
        }
    }
}
