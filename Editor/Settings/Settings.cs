using UnityEditor;
using UnityEngine;

namespace ServerSync.Editor
{
    public enum WindowStyle
    {
        [Tooltip("The window is minimized")]
        Minimized,

        [Tooltip("The window is completely hidden")]
        Hidden
    }

    /// <summary>
    /// Project-specific settings.
    /// </summary>
    [FilePath(FilePath, FilePathAttribute.Location.ProjectFolder)]
    class Settings : ScriptableSingleton<Settings>
    {
        const string FilePath = "ServerSync/ServerSync.settings";

        public bool IsEnabled => m_IsEnabled;

        public string BuildPath => m_BuildPath;

        public WindowStyle WindowStyle => m_WindowStyle;

        public bool ExcludeShadersFromBuild => m_ExcludeShadersFromBuild;

        public bool WaitForNotify => m_WaitForNotify;

        public int WaitForNotifyPort => m_WaitForNotifyPort;

        public float WaitForNotifyTimeout => m_WaitForNotifyTimeout;

        [Tooltip("Enables/disables this package.")]
        [SerializeField]
        bool m_IsEnabled = true;

        [Tooltip("The path where the server build should be stored (relative to the project root).")]
        [SerializeField]
        string m_BuildPath = "ServerSyncBuild";

        [Tooltip("The style to apply onto the server window.")]
        [SerializeField]
        WindowStyle m_WindowStyle = WindowStyle.Minimized;

        [Tooltip("Exclude shaders from the build to speed it up (recommended).")]
        [SerializeField]
        bool m_ExcludeShadersFromBuild = true;

        [Tooltip("If true, the editor will wait until the server notifies that it's ready before entering Play mode")]
        [SerializeField]
        bool m_WaitForNotify;

        [Tooltip("The localhost port to use for the notification")]
        [SerializeField]
        int m_WaitForNotifyPort = WaitNotifierSettings.DefaultPort;

        [Tooltip("Max time for the editor to wait to be notified by the server. No timeout if the value is set to 0.")]
        [SerializeField]
        float m_WaitForNotifyTimeout;

        public void SaveChanges()
        {
            Save(true);
        }

        public void Reset()
        {
            m_IsEnabled = true;
            m_BuildPath = "ServerSyncBuild";
            m_WindowStyle = WindowStyle.Minimized;
            m_ExcludeShadersFromBuild = true;
            m_WaitForNotify = false;
            m_WaitForNotifyPort = WaitNotifierSettings.DefaultPort;
            m_WaitForNotifyTimeout = 0f;

            SaveChanges();
        }
    }
}
