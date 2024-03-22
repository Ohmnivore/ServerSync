using System.Diagnostics;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace ServerSync.Editor
{
    /// <summary>
    /// Hooks into the Unity lifecycle.
    /// </summary>
    public class EntryPoint
    {
        [InitializeOnLoadMethod]
        private static void EditorInitialize()
        {
            EditorApplication.playModeStateChanged += OnplayModeStateChanged;
        }

        private static void OnplayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (!Settings.instance.IsEnabled)
                return;

            if (stateChange == PlayModeStateChange.ExitingEditMode)
            {
                var buildReport = Builder.instance.Build();

                if (buildReport.summary.result == BuildResult.Succeeded)
                {
                    var windowStyle = Settings.instance.WindowStyle == WindowStyle.Minimized ?
                        ProcessWindowStyle.Minimized :
                        ProcessWindowStyle.Hidden;

                    var startInfo = new ProcessStartInfo
                    {
                        FileName = Builder.instance.BuildLocation,
                        WindowStyle = windowStyle
                    };
                    Runner.instance.Launch(startInfo, OnRunCancel, OnRunTimeout);
                }
                else
                {
                    EditorApplication.ExitPlaymode();

                    if (buildReport.summary.result == BuildResult.Failed)
                        UnityEngine.Debug.LogError("ServerSync: Server build failed");
                }
            }
            else if (stateChange == PlayModeStateChange.ExitingPlayMode)
            {
                Runner.instance.Stop();
            }
        }

        private static void OnRunCancel()
        {
            Runner.instance.Stop();
            EditorApplication.ExitPlaymode();
        }

        private static void OnRunTimeout()
        {
            UnityEngine.Debug.LogError("ServerSync: Timed out while waiting for server to get ready and notify " +
                                       "(using ServerSync.WaitNotifier.Instance.NotifyReady)");

            Runner.instance.Stop();
            EditorApplication.ExitPlaymode();
        }
    }
}
