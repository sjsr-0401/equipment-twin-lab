using System;
using System.IO;
using UnityEngine;

namespace EquipmentTwin.Unity.Processes
{
    public static class MolyAldTimelineLoader
    {
        public const string SupportedSchemaVersion = "equipment-twin.moly-ald.timeline.v1";

        public static MolyAldTimelineDocumentDto FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Timeline JSON is empty.", nameof(json));
            }

            var timeline = JsonUtility.FromJson<MolyAldTimelineDocumentDto>(json);
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
