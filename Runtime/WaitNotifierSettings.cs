using UnityEngine;

namespace ServerSync
{
    /// <summary>
    /// Settings for <see cref="WaitNotifier"/>.
    /// </summary>
    public class WaitNotifierSettings : ScriptableObject
    {
        /// <summary>
        /// The default localhost port to use for notifying the editor.
        /// </summary>
        public const int DefaultPort = 3030;

        /// <summary>
        /// The Resource path of the <see cref="WaitNotifierSettings"/> asset included in ServerSync server builds.
        /// </summary>
        public const string Path = "ServerSync WaitNotifierSettings";

        /// <summary>
        /// The localhost port to use for notifying the editor.
        /// </summary>
        public int Port = DefaultPort;
    }
}
