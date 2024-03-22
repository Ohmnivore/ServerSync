#if SERVER_SYNC
using System.Net.Http;
using UnityEngine;

namespace ServerSync
{
    /// <summary>
    /// Notifies <see cref="ServerSync.Editor.WaitListener"/>.
    /// Call <see cref="NotifyReady"/> on the server once it's ready to accept clients.
    /// </summary>
    public class WaitNotifier
    {
        private const int Timeout = 3000;

        private static WaitNotifier instance;

        public static WaitNotifier Instance => instance ??= new WaitNotifier();

        /// <summary>
        /// Call this function on the server once it's ready to accept clients.
        /// The editor will wait for this call before entering Play mode.
        /// </summary>
        public void NotifyReady()
        {
            var settings = Resources.Load<WaitNotifierSettings>(WaitNotifierSettings.Path);
            var uri = $"http://localhost:{settings.Port}/";

            using var client = new HttpClient();
            var task = client.GetAsync(uri);
            task.Wait(Timeout);

            if (!task.IsCompletedSuccessfully)
                Debug.Log($"ServerSync: Failed to notify editor");
        }
    }
}
#endif
