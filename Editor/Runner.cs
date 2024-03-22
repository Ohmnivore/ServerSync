using System;
using System.Diagnostics;
using Hextant.Editor;
using UnityEngine;

namespace ServerSync.Editor
{
    /// <summary>
    /// Manages the ServerSync server executable.
    /// </summary>
    public class Runner : EditorSingleton<Runner>
    {
        // Only works before we enter Play mode
        internal bool IsRunning => m_Process is { HasExited: false };

        // Doesn't survive domain reload when entering/exiting Play mode
        private Process m_Process;

        // Survives domain reload when entering/exiting Play mode
        [SerializeField]
        private int ProcessId;

        /// <summary>
        /// Launches the server executable.
        /// </summary>
        /// <param name="startInfo">Process configuration.</param>
        /// <param name="onCancel">Called if the user cancels the operation while waiting for the server to be ready.</param>
        /// <param name="onTimeout">Called if timed out while waiting for the server to be ready.</param>
        public void Launch(ProcessStartInfo startInfo, Action onCancel, Action onTimeout)
        {
            m_Process = new Process();
            m_Process.StartInfo = startInfo;
            m_Process.Start();

            ProcessId = m_Process.Id;

            if (Settings.instance.WaitForNotify)
                WaitListener.instance.Listen(onCancel, onTimeout);
        }

        /// <summary>
        /// Kills the server executable.
        /// </summary>
        public void Stop()
        {
            if (ProcessId > 0)
            {
                var process = Process.GetProcessById(ProcessId);
                if (process is { HasExited: false })
                {
                    process.Kill();
                    process.WaitForExit();
                }
            }

            ProcessId = 0;
        }
    }
}
