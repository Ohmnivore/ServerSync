using System;
using System.Net;
using Hextant.Editor;
using UnityEditor;
using UnityEngine;

namespace ServerSync.Editor
{
    /// <summary>
    /// Waits for <see cref="ServerSync.WaitNotifier"/> (blocking the main thread) before entering Play mode.
    /// </summary>
    public class WaitListener : EditorSingleton<WaitListener>
    {
        private HttpListener m_Listener;
        private bool m_WasCancelled;
        private bool m_WasNotified;

        public void Listen(Action onCancel, Action onTimeout)
        {
            m_WasCancelled = false;
            m_WasNotified = false;

            m_Listener = new HttpListener();
            m_Listener.Prefixes.Add($"http://localhost:{Settings.instance.WaitForNotifyPort}/");
            m_Listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            m_Listener.Start();

            var timeoutEnabled = Settings.instance.WaitForNotifyTimeout > 0f;
            var startTime = EditorApplication.timeSinceStartup;
            var endTime = startTime + Settings.instance.WaitForNotifyTimeout;

            while (!m_WasNotified && (!timeoutEnabled || EditorApplication.timeSinceStartup < endTime))
            {
                // Was closed externally by the user, ex if server failed to start
                if (!Runner.instance.IsRunning)
                {
                    Debug.Log("ServerSync: Server process was closed externally, cancelling Play mode");

                    m_WasCancelled = true;
                    onCancel?.Invoke();
                    break;
                }

                var progress = timeoutEnabled ?
                    (EditorApplication.timeSinceStartup - startTime) / (endTime - startTime) :
                    -1;
                if (EditorUtility.DisplayCancelableProgressBar("ServerSync", "Waiting for server to get ready and notify...", (float)progress))
                {
                    m_WasCancelled = true;
                    onCancel?.Invoke();
                    break;
                }

                var result = m_Listener.BeginGetContext(ListenerCallback, m_Listener);
                result.AsyncWaitHandle.WaitOne(30, true);
            }
            EditorUtility.ClearProgressBar();

            m_Listener.Close();
            m_Listener = null;

            if (!m_WasNotified && !m_WasCancelled)
                onTimeout?.Invoke();
        }

        private void ListenerCallback(IAsyncResult result)
        {
            HttpListenerContext context = m_Listener.EndGetContext(result);
            context.Response.Close();
            m_WasNotified = true;
        }
    }
}
