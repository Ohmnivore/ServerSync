using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Hextant.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace ServerSync.Editor
{
    /// <summary>
    /// Manages the server build used for ServerSync.
    /// </summary>
    public class Builder : EditorSingleton<Builder>
    {
        private struct BuildScope : IDisposable
        {
            private Builder Builder;
            
            public BuildScope(Builder builder)
            {
                Builder = builder;
                Builder.IsBuilding = true;
            }

            public void Dispose()
            {
                Builder.IsBuilding = false;
            }
        }

        /// <summary>
        /// Added to the server build scripting define symbols (using extraScriptingDefines).
        /// </summary>
        public const string SERVER_SYNC_DEFINE_SYMBOL = "SERVER_SYNC";

        /// <summary>
        /// The path of the ServerSync server build executable.
        /// </summary>
        public string BuildLocation => EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget);

        /// <summary>
        /// True when the ServerSync server build is being built.
        /// </summary>
        public bool IsBuilding { get; private set; }

        // BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptionsInternal is like
        // BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions
        // but it lets us skip the file selection dialog
        private MethodInfo GetBuildPlayerOptionsInternal;

        private void InitializeReflectionInfo()
        {
            if (GetBuildPlayerOptionsInternal != null)
                return;

            GetBuildPlayerOptionsInternal =
                typeof(BuildPlayerWindow.DefaultBuildMethods).GetMethod(nameof(GetBuildPlayerOptionsInternal),
                    BindingFlags.Static | BindingFlags.NonPublic);

            if (GetBuildPlayerOptionsInternal == null)
                throw new Exception("Failed to resolve BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptionsInternal");
        }

        /// <summary>
        /// Builds the ServerSync server build.
        /// </summary>
        public BuildReport Build()
        {
            InitializeReflectionInfo();

            EditorUserBuildSettings.SwitchActiveBuildTarget(NamedBuildTarget.Server, BuildTarget.StandaloneWindows64);

            var path = Path.GetFullPath(Settings.instance.BuildPath);
            var fileName = Path.ChangeExtension(PlayerSettings.productName, ".exe");
            var executablePath = Path.Join(path, fileName);
            EditorUserBuildSettings.SetBuildLocation(EditorUserBuildSettings.activeBuildTarget, executablePath);

            var buildPlayerOptions = (BuildPlayerOptions)GetBuildPlayerOptionsInternal.Invoke(null, new object[]{ false, new BuildPlayerOptions() });

            var extraScriptingDefines = new[] { SERVER_SYNC_DEFINE_SYMBOL };
            buildPlayerOptions.extraScriptingDefines = buildPlayerOptions.extraScriptingDefines == null ?
                extraScriptingDefines :
                buildPlayerOptions.extraScriptingDefines.Concat(new[] { SERVER_SYNC_DEFINE_SYMBOL }).ToArray();

            using var buildScope = new BuildScope(this);
            return BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}
