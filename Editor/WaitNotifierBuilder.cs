using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ServerSync.Editor
{
    /// <summary>
    /// Adds a temporary configuration asset for the SyncServer build.
    /// </summary>
    public class WaitNotifierPreBuilder : IPreprocessBuildWithReport
    {
        /// <summary>
        /// The AssetDatabase path of the folder containing the temporary configuration asset.
        /// </summary>
        public const string SettingsDirectory = "Assets/ServerSync/Resources";

        /// <summary>
        /// The AssetDatabase path of the temporary configuration asset.
        /// </summary>
        public static readonly string AssetPath = $"{SettingsDirectory}/{WaitNotifierSettings.Path}.asset";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!Builder.instance.IsBuilding)
                return;

            if (!Settings.instance.WaitForNotify)
                return;

            var settings = ScriptableObject.CreateInstance<WaitNotifierSettings>();
            settings.Port = Settings.instance.WaitForNotifyPort;

            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetDirectoryName(AssetPath));
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            AssetDatabase.CreateAsset(settings, AssetPath);
            AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
    /// Deletes the configuration asset (it was only intended for the SyncServer server build).
    /// </summary>
    public class WaitNotifierPostBuilder : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (!Builder.instance.IsBuilding)
                return;

            if (!Settings.instance.WaitForNotify)
                return;

            if (AssetDatabase.DeleteAsset(WaitNotifierPreBuilder.AssetPath))
            {
                if (AssetDatabase.DeleteAsset(WaitNotifierPreBuilder.SettingsDirectory))
                {
                    // Delete all the created folders
                    AssetDatabase.DeleteAsset(Path.GetDirectoryName(WaitNotifierPreBuilder.SettingsDirectory));
                }
            }
        }
    }
}
